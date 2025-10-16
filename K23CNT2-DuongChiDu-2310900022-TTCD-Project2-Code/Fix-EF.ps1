Write-Host "🚀 Gỡ toàn bộ EF Core cũ khỏi project..."
dotnet remove package Microsoft.EntityFrameworkCore
dotnet remove package Microsoft.EntityFrameworkCore.SqlServer
dotnet remove package Microsoft.EntityFrameworkCore.Tools
dotnet remove package Microsoft.EntityFrameworkCore.Design

Write-Host "📦 Cài lại EF Core 8.0.6..."
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.6
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.6
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.6
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.6

Write-Host "🛠 Gỡ code generator bản sai..."
dotnet tool uninstall --global dotnet-aspnet-codegenerator

Write-Host "🛠 Cài lại code generator bản đúng cho .NET 8..."
dotnet tool install --global dotnet-aspnet-codegenerator --version 8.0.3

Write-Host "🧹 Xóa cache NuGet..."
dotnet nuget locals all --clear

Write-Host "🧹 Clean + Restore + Build project..."
dotnet clean
dotnet restore
dotnet build

Write-Host "✅ Scaffold 4 controller CRUD..."

dotnet aspnet-codegenerator controller -name OrdersController -m Order -dc AppDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
dotnet aspnet-codegenerator controller -name OrderDetailsController -m OrderDetail -dc AppDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
dotnet aspnet-codegenerator controller -name PaymentsController -m Payment -dc AppDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
dotnet aspnet-codegenerator controller -name ReviewsController -m Review -dc AppDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

Write-Host "🎉 Hoàn tất! Controllers và Views đã tạo trong thư mục Controllers/ và Views/."
