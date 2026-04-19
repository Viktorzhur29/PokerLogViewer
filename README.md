# Poker Log Viewer

Приложение для просмотра и анализа покерных логов в формате JSON. Разработано в рамках тестового задания.

## Требования

- **.NET 8.0 SDK** ([скачать](https://dotnet.microsoft.com/download/dotnet/8.0))
- **CMake 3.27** или выше ([скачать](https://cmake.org/download/))
- **Visual Studio 2022/2026** с установленной поддержкой .NET
- **Windows 10/11 x64**

## Структура проекта
PokerLogViewer/
├── CMakeLists.txt # Файл сборки CMake
├── README.md # Документация
├── .gitignore # Исключения для Git
├── sample_data/ # Тестовые JSON файлы
└── src/
└── PokerLogViewer/
├── PokerLogViewer.csproj
├── Models/ # Модели данных
├── Services/ # Сервис сканирования
├── ViewModels/ # ViewModel для MVVM
├── Views/ # WPF представления
└── Converters/ # Конвертеры для XAML
## Сборка проекта

### Сборка через CMake


### Конфигурация и генерация решения
cmake -B build -G "Visual Studio 18 2026" -A x64

### Сборка Release версии
cmake --build build --config Release
Для Visual Studio 2022 используйте генератор "Visual Studio 17 2022".

Сборка через dotnet CLI
bash
### Перейти в папку с проектом
cd src/PokerLogViewer

### Восстановить зависимости
dotnet restore

### Собрать проект
dotnet build -c Release

### Запустить приложение
dotnet run -c Release

Публикация в один исполняемый файл

bash
dotnet publish src/PokerLogViewer/PokerLogViewer.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish

Исполняемый файл 
PokerLogViewer.exe будет создан в папке publish/.