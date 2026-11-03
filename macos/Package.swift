// swift-tools-version: 5.9
import PackageDescription

let package = Package(
    name: "KimiPocket",
    platforms: [
        .macOS(.v13)
    ],
    products: [
        .executable(name: "KimiPocket", targets: ["KimiPocket"]),
        .executable(name: "TestRunner", targets: ["TestRunner"])
    ],
    dependencies: [],
    targets: [
        .target(
            name: "KimiPocketCore",
            path: "Sources/Core"
        ),
        .executableTarget(
            name: "KimiPocket",
            dependencies: ["KimiPocketCore"],
            path: "Sources/App"
        ),
        .executableTarget(
            name: "TestRunner",
            dependencies: ["KimiPocketCore"],
            path: "Tests/TestRunner"
        )
    ]
)
