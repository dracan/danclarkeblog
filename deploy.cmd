
@echo off
echo Deploying webjob files ...
echo copying %DEPLOYMENT_TARGET%\DanClarkeBlog.Tasks\bin\Release\*
echo to %DEPLOYMENT_TARGET%\DanClarkeBlog.Web\bin\Release\netcoreapp1.1\publish\App_Data\jobs\triggered\TestJob\
xcopy /Y %DEPLOYMENT_TARGET%\DanClarkeBlog.Tasks\bin\Release\* %DEPLOYMENT_TARGET%\DanClarkeBlog.Web\bin\Release\netcoreapp1.1\publish\App_Data\jobs\triggered\TestJob\
