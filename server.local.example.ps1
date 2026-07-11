# Diese Datei nach server.local.ps1 kopieren und nur lokal ausfüllen.
# server.local.ps1 wird durch .gitignore nicht veröffentlicht.

$env:KCRIMELIFE_DB_HOST = "localhost"
$env:KCRIMELIFE_DB_PORT = "3306"
$env:KCRIMELIFE_DB_NAME = "kcrimelife"
$env:KCRIMELIFE_DB_USER = "root"
$env:KCRIMELIFE_DB_PASSWORD = "HIER_DEIN_DATENBANKPASSWORT"
$env:KCRIMELIFE_WHITELIST = "false"

# Optional: ein gemeinsamer Discord-Webhook für alle Logs.
# Einzelne Kanäle lassen sich mit Variablen wie
# KCRIMELIFE_WEBHOOK_ADMINLOGS oder KCRIMELIFE_WEBHOOK_JOINLOGS setzen.
$env:KCRIMELIFE_WEBHOOK_DEFAULT = ""
