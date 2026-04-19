# Poker Log Viewer

Приложение для просмотра и анализа покерных логов в формате JSON.

## Требования
- .NET 8.0 SDK
- Visual Studio 2022 (или 2019) с установленной поддержкой .NET
- CMake 3.27 или выше
- Windows 10/11 x64

## Сборка проекта

### Через CMake
```bash
# Конфигурация
cmake -B build -G "Visual Studio 17 2022"

# Сборка
cmake --build build --config Release