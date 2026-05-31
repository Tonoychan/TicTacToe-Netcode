<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:1a1a2e,50:16213e,100:0f3460&height=200&section=header&text=TicTacToe%20Netcode&fontSize=52&fontColor=ffffff&fontAlignY=38&desc=Multiplayer%20TicTacToe%20%E2%80%A2%20Unity%20Netcode%20for%20GameObjects&descAlignY=58&descSize=18&animation=fadeIn" width="100%"/>

<br/>

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Netcode](https://img.shields.io/badge/Netcode%20for%20GameObjects-0f3460?style=for-the-badge&logo=unity&logoColor=white)
![Multiplayer](https://img.shields.io/badge/Multiplayer-Online-e94560?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Complete-brightgreen?style=for-the-badge)

</div>

---

## 📖 About This Project

> ⚠️ **Learning Project** — This is a hands-on recreation of the **[Code Monkey](https://www.youtube.com/@CodeMonkeyUnity)** YouTube tutorial:
> **"Simplest Multiplayer Game! (FREE Course — Unity Netcode for Game Objects)"**
> Redone from scratch following his guidance to deeply understand **Netcode for GameObjects**, Unity's official multiplayer networking solution.

A fully functional **online multiplayer Tic-Tac-Toe** game built with Unity and Netcode for GameObjects. Two players connect over a network, take turns placing their marks, and the first to get three in a row wins. Simple game design — serious networking knowledge.

---

## 🎮 What I Learned

Following Code Monkey's tutorial, this project covers the key pillars of Unity multiplayer development:

| Concept | What It Does |
|---|---|
| `NetworkManager` | Manages host/client connections and the network lifecycle |
| `NetworkObject` | Marks GameObjects as network-aware and synced across clients |
| `NetworkVariable<T>` | Syncs state automatically between host and all clients |
| `ServerRpc` | Client → Server calls (e.g. player makes a move) |
| `ClientRpc` | Server → All Clients calls (e.g. broadcast game state) |
| Lobby System | Players join a shared session before the game starts |
| Turn Management | Server-authoritative turn switching between two players |
| Game State Machine | Tracks WaitingForPlayers → Playing → GameOver |
| Winner Detection | Server checks all win conditions after each move |
| Rematch System | Players vote to restart without leaving the session |

---

## 🏗️ Project Architecture

```
Assets/
├── Scripts/
│   ├── GameManager.cs          # Core game logic, state machine, win detection
│   ├── TicTacToeGrid.cs        # Board state with NetworkVariables
│   ├── PlayerController.cs     # Input handling + ServerRpc for placing marks
│   ├── LobbyManager.cs         # Host/join lobby UI and connection flow
│   ├── NetworkUI.cs            # UI bindings for multiplayer state
│   └── GameOverUI.cs           # Win/draw screen + rematch voting
├── Prefabs/
│   ├── NetworkManager.prefab   # Configured Netcode NetworkManager
│   └── GridCell.prefab         # Individual board cell (NetworkObject)
└── Scenes/
    ├── LobbyScene.unity
    └── GameScene.unity
```

---

## 🌐 How The Multiplayer Works

```
Player 1 (Host)                     Player 2 (Client)
      |                                     |
      |── Start Host ──────────────────────>|
      |                                     |── Join via IP/Code
      |<─────────────── Connected ──────────|
      |                                     |
      |   [Player 2 clicks a cell]          |
      |<────── PlaceMark_ServerRpc() ───────|
      |                                     |
      |── Validate move (server authority)  |
      |── Update NetworkVariable (board)    |
      |── UpdateBoard_ClientRpc() ─────────>|
      |                                     |
      |── Check win condition               |
      |── Switch turn NetworkVariable ─────>|
```

---

## 🛠️ Tech Stack

| Area | Technology |
|---|---|
| Engine | Unity |
| Language | C# |
| Networking | Netcode for GameObjects (`com.unity.netcode.gameobjects`) |
| Transport | Unity Transport (`com.unity.transport`) |
| Services | Unity Gaming Services (Lobby, Relay) |
| UI | Unity uGUI · TextMesh Pro |
| IDE | JetBrains Rider |

---

## 🚀 Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download)
- Unity Editor with **Netcode for GameObjects** package installed
- Two instances of the game (or two machines on the same network)

### Setup

```bash
# 1. Clone the repo
git clone https://github.com/Tonoychan/TicTacToe-Netcode.git

# 2. Open in Unity Hub (select the repo root folder)

# 3. Let packages resolve — Netcode & Transport will auto-install

# 4. Open GameScene and hit Play ▶
```

### Playing Locally (Two Instances)

```
Instance 1 → Click "Host"
Instance 2 → Enter localhost / IP → Click "Join"
```

---

## 🗺️ Roadmap

- [x] Unity project setup with Netcode for GameObjects
- [x] NetworkManager configuration (host / client / server modes)
- [x] Board grid with NetworkVariables for synced state
- [x] ServerRpc for player move validation
- [x] ClientRpc for broadcasting board updates
- [x] Turn management (server-authoritative)
- [x] Win condition detection
- [x] Draw condition detection
- [x] Game Over UI with winner display
- [x] Rematch system
- [x] Lobby / connection UI

---

## 🙏 Credits

Tutorial by **[Code Monkey](https://www.youtube.com/@CodeMonkeyUnity)** —
*"Simplest Multiplayer Game! FREE Course — Unity Netcode for Game Objects"*
This project was redone from scratch following his guidance to build a solid understanding of Netcode for GameObjects.

---

## 👨‍💻 Author

**Tonoy Chakraborty**

---

<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:0f3460,50:16213e,100:1a1a2e&height=120&section=footer" width="100%"/>

*Built with Unity · Learned with Code Monkey · Networked with ❤️*

</div>
