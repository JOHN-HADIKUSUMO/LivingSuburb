https://long2know.com/2018/02/net-core-console-app-dependency-injection-and-user-secrets 
https://www.pmichaels.net/2019/01/08/creating-a-windows-service-using-net-core-2-2

sc delete "Politic Service" binpath=D:\Test\PoliticService\LivingSuburb.PoliticWinService.exe
sc create "Politic Service" binpath=D:\Test\PoliticService\LivingSuburb.PoliticWinService.exe
sc start "Politic Service"