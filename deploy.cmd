@echo off
IF "%SITE_FLAVOR%" == "webapp" (
  deploy.webapp.cmd
) ELSE (
  IF "%SITE_FLAVOR%" == "functions" (
    deploy.functions.cmd
  ) ELSE (
    echo You have to set SITE_FLAVOR setting to either "webapp" or "functions"
    exit /b 1
  )
)