# Library Management System - Independent overdue borrowing test
# Place this file in the same folder as LibraryManagementSystem.csproj
# Run: powershell -NoProfile -ExecutionPolicy Bypass -File .\Test_Overdue_Loans.ps1
# This script NEVER changes the original C# files.

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$testRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("LibraryOverdueTest_" + [guid]::NewGuid().ToString('N'))
$resultsRoot = Join-Path $projectRoot 'LibraryTestResults'
$outputPath = Join-Path $resultsRoot 'Overdue_Independent.actual.txt'
$errorPath = Join-Path $resultsRoot 'Overdue_Independent.errors.txt'
$utf8 = New-Object System.Text.UTF8Encoding($false)

try {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw 'dotnet SDK was not found.'
    }

    $projectFiles = @(Get-ChildItem -LiteralPath $projectRoot -Filter '*.csproj' -File)
    if ($projectFiles.Count -ne 1) {
        throw 'Put this script in the project directory containing exactly one .csproj file.'
    }

    [System.IO.Directory]::CreateDirectory($testRoot) | Out-Null
    [System.IO.Directory]::CreateDirectory($resultsRoot) | Out-Null

    # Copy project source into an isolated temporary directory.
    Copy-Item -LiteralPath $projectFiles[0].FullName -Destination $testRoot
    foreach ($folderName in @('Models', 'Services', 'Menus')) {
        $source = Join-Path $projectRoot $folderName
        if (Test-Path -LiteralPath $source) {
            Copy-Item -LiteralPath $source -Destination $testRoot -Recurse -Force
        }
    }

    # Include other root C# source files, excluding the original Program.cs.
    Get-ChildItem -LiteralPath $projectRoot -Filter '*.cs' -File |
        Where-Object { $_.Name -ne 'Program.cs' } |
        ForEach-Object {
            Copy-Item -LiteralPath $_.FullName -Destination $testRoot -Force
        }

    # Standalone test runner, only inside the temporary project copy.
    $program = @'
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

int failures = 0;

void Check(string label, bool passed)
{
    Console.WriteLine($"{(passed ? "PASS" : "FAIL")} : {label}");
    if (!passed)
        failures++;
}

Library library = new Library();
FineService fineService = new FineService();
LoanService loanService = new LoanService(library, fineService);

for (int i = 1; i <= 5; i++)
{
    library.AddItem(new Book(i, $"Book {i}", "Author", 100));
}

library.AddMember(new Member(501, "Overdue Member", "overdue501", 3));
library.AddMember(new Member(502, "On Time Member", "ontime502", 3));

// Book 1 is 1 day overdue (14-day lending period, borrowed 15 days ago).
Loan? lateLoan = loanService.BorrowItem(501, 1, DateTime.Today.AddDays(-15));
Check("Old loan created", lateLoan != null);
Check("Loan is currently overdue", lateLoan != null && lateLoan.IsOverdue(DateTime.Today));

Loan? blocked = loanService.BorrowItem(501, 2);
Check("Overdue member cannot borrow another book", blocked == null);

// Another member can borrow; due TODAY does not count as overdue.
Loan? dueToday = loanService.BorrowItem(502, 2, DateTime.Today.AddDays(-14));
Check("Other member loan created", dueToday != null);
Check("Due today is NOT overdue", dueToday != null && !dueToday.IsOverdue(DateTime.Today));

Loan? allowed = loanService.BorrowItem(502, 3);
Check("Member with loan due today CAN borrow", allowed != null);

Check("Overdue report shows exactly one loan", loanService.GetOverdueLoans(DateTime.Today).Count == 1);

if (lateLoan != null)
{
    bool returned = loanService.ReturnItem(lateLoan.Id, DateTime.Today);
    Check("Overdue book returned successfully", returned);
    Check("Exactly one NIS fine created", fineService.GetMemberUnpaidTotal(501) == 1m);
    Check("Returned loan no longer overdue", loanService.GetOverdueLoans(DateTime.Today).Count == 0);

    // Returning a book does not erase an unpaid fine.
    Loan? blockedByFine = loanService.BorrowItem(501, 4);
    Check("Unpaid fine still blocks borrowing", blockedByFine == null);

    List<Fine> memberFines = fineService.GetMemberFines(501);
    if (memberFines.Count == 1)
    {
        Fine fine = memberFines[0];
        bool paid = fineService.PayFine(
            fine.Id,
            fine.GetRemainingAmount(),
            "OVERDUE-TEST-PAY-001"
        );
        Check("Full fine payment accepted", paid);
    }
    else
    {
        Check("One fine record exists", false);
    }

    Loan? afterPayment = loanService.BorrowItem(501, 4);
    Check("Borrowing allowed after returning and paying", afterPayment != null);
}

Check("Active loans remain correctly tracked", loanService.GetActiveLoans().Count == 3);
Console.WriteLine($"TOTAL FAILED: {failures}");
Environment.ExitCode = failures == 0 ? 0 : 1;
'@

    [System.IO.File]::WriteAllText((Join-Path $testRoot 'Program.cs'), $program, $utf8)

    # Use the process API for a reliable exit code in Windows PowerShell.
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = 'dotnet'
    $psi.Arguments = 'run --no-launch-profile'
    $psi.WorkingDirectory = $testRoot
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true

    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $psi
    $null = $process.Start()
    $outTask = $process.StandardOutput.ReadToEndAsync()
    $errTask = $process.StandardError.ReadToEndAsync()

    if (-not $process.WaitForExit(120000)) {
        try { $process.Kill() } catch {}
        throw 'Test timed out after 120 seconds.'
    }

    $exitCode = $process.ExitCode
    $output = $outTask.GetAwaiter().GetResult()
    $errors = $errTask.GetAwaiter().GetResult()
    $process.Dispose()

    [System.IO.File]::WriteAllText($outputPath, $output, $utf8)
    [System.IO.File]::WriteAllText($errorPath, $errors, $utf8)

    Write-Host $output
    Write-Host "Exit code: $exitCode"
    Write-Host "Full output: $outputPath"

    if ($exitCode -ne 0 -or $output -match 'FAIL\s*:') {
        Write-Host 'OVERDUE TEST FAILED. Check .actual.txt and .errors.txt.' -ForegroundColor Red
        exit 1
    }

    Write-Host 'ALL OVERDUE TESTS PASSED' -ForegroundColor Green
}
catch {
    Write-Host ("TEST ERROR: " + $_.Exception.Message) -ForegroundColor Red
    exit 1
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force -ErrorAction SilentlyContinue
    }
}
