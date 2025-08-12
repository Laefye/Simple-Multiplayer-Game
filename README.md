# Simple Multiplayer Game

A Unity-based multiplayer game with TCP networking capabilities.

## Architecture Overview

This project has been refactored to provide a clean, maintainable, and robust codebase for multiplayer gaming.

### Core Components

#### Network Layer (`Assets/Scripts/Net/`)

- **NetworkManager**: Central manager coordinating client and server operations
- **ClientNetworkManager**: Handles client-side networking and packet reception
- **ServerNetworkManager**: Manages server-side connections and packet distribution
- **Connection**: Thread-safe TCP connection wrapper with proper resource disposal
- **Packet**: Binary serialization system for network data
- **NetworkConstants**: Centralized constants and channel definitions
- **NetworkConfig**: Scriptable object for runtime configuration
- **NetworkValidator**: Input validation and data sanitization utilities

#### Player Management

- **Player**: Base player component with ID and username
- **ClientPlayer**: Local player with movement controls and network sync
- **ServerPlayer**: Server-side player representation with network communication
- **ShadowPlayer**: Remote player representation for other clients

#### Packet Handlers

- **SyncServerPlayer**: Handles player position/rotation updates
- **ClientShadowPlayer**: Manages remote player state on clients
- **ClientShadowPlayerDisconnect**: Handles player disconnection cleanup
- **ServerGetAllPlayers**: Provides current player list to new connections

### Key Improvements Made

#### 1. **Code Organization & Documentation**
- Added comprehensive XML documentation for all public APIs
- Organized code into logical namespaces
- Removed unused imports and dependencies
- Consistent naming conventions

#### 2. **Error Handling & Robustness**
- Try-catch blocks around all network operations
- Graceful handling of connection failures
- Proper resource disposal with IDisposable pattern
- Validation of network input data

#### 3. **Performance Optimizations**
- Player caching system to avoid repeated object searches
- Replaced LINQ operations with more efficient loops
- Thread-safe collections for cross-thread operations
- Reduced object allocation in hot paths

#### 4. **Network Security & Validation**
- Username sanitization and validation
- Packet size limits to prevent memory issues
- Position/rotation validation to prevent invalid data
- Connection limits and timeouts

#### 5. **Configuration System**
- Centralized constants for easy modification
- Scriptable object configuration for runtime settings
- Configurable network parameters
- Organized channel definitions

#### 6. **Thread Safety**
- ConcurrentQueue for thread-safe action queuing
- Proper synchronization between network and Unity threads
- Safe disposal of network resources

### Usage

#### Starting a Server
```csharp
// In ConnectTool or custom script
networkManager.Server.Listen(); // Uses default port 2228
// or
networkManager.Server.Listen(customPort);
```

#### Connecting as Client
```csharp
// In ConnectTool or custom script
networkManager.Client.Connect(serverIP, username);
```

#### Configuration
Create a `NetworkConfig` asset to customize:
- Network ports and buffer sizes
- Player spawn positions
- Movement settings
- Update rates

### Network Protocol

The game uses a simple TCP-based protocol with the following packet structure:
1. Channel identifier (string)
2. Packet data (varies by channel type)

#### Supported Channels
- `sync_server_player`: Player position/rotation updates
- `shadow_player_sync`: Remote player state synchronization
- `shadow_player_disconnect`: Player disconnection notifications
- `get_all_players`: Request for current player list

### Best Practices Implemented

1. **Resource Management**: All network resources properly disposed
2. **Error Recovery**: Graceful handling of network failures
3. **Input Validation**: All user input sanitized and validated
4. **Performance**: Optimized for real-time multiplayer scenarios
5. **Maintainability**: Well-documented and organized codebase
6. **Security**: Basic protection against malformed packets

### Future Enhancements

Potential areas for further improvement:
- UDP support for real-time game data
- Client-side prediction and lag compensation
- Encryption for secure communications
- Dedicated server architecture
- Matchmaking system
- Player authentication

---

This refactored codebase provides a solid foundation for multiplayer game development with Unity, emphasizing clean architecture, performance, and maintainability.