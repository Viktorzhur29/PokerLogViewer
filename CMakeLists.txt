cmake_minimum_required(VERSION 3.27)
project(PokerLogViewer NONE)

add_custom_target(PokerLogViewer ALL
    COMMAND dotnet build ${CMAKE_CURRENT_SOURCE_DIR}/src/PokerLogViewer/PokerLogViewer.csproj -c Release
    COMMENT "Building PokerLogViewer with dotnet"
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}
)

add_custom_target(run
    COMMAND dotnet run --project ${CMAKE_CURRENT_SOURCE_DIR}/src/PokerLogViewer/PokerLogViewer.csproj -c Release
    COMMENT "Running PokerLogViewer"
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}
)
