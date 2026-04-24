# 🚗 TurboStorm

TurboStorm is a high-speed racing game built with Unity, focused on immersive environments, responsive vehicle physics, and modular gameplay systems.

This repository excludes several large Unity asset packages due to GitHub size limitations. You must install them manually for the project to function correctly.

---

[![Watch the video](https://i.sstatic.net/Vp2cE.png)](https://youtu.be/vt5fpE0bzSY)


## 📦 Missing Assets (Required Setup)

The following Unity asset packs are **NOT included** in this repository and must be installed manually:

* Coconut Palm Tree Pack
* Forst
* RoadArchitect
* BxB Studio
* Flooded_Grounds
* Rowlan
* UK Terraced Houses FREE
* Versatile Studio Assets
* Fantasy Forest Environment Free Sample

### 🚗 Vehicle Physics (Required)

One of the following vehicle controller plugins is required:

* **Multiversal Vehicle Controller: Pro - Car Physics Controller**
* **Multiversal Vehicle Controller: Community - Car Physics Controller**

> Either version will work. The **Community version is sufficient**, while the **Pro version offers advanced tuning and features**.

---

## ⚙️ Installation Guide

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/turbostorm.git
cd turbostorm
```

---

### 3. Import Missing Assets

#### Option A — Unity Asset Store

1. Open Unity
2. Go to **Window → Asset Store** (or use browser)
3. Search each asset by name
4. Download and import

## ▶️ Running the Game

1. Open the main scene:

   ```
   Assets/Scenes/scene1.unity
   ```

2. Press **Play** in the Unity Editor

---

## 🎮 Controls (Default)

| Action     | Key                   |
| ---------- | --------------------- |
| Accelerate | W / Up Arrow          |
| Brake      | S / Down Arrow        |
| Steer      | A / D or Left / Right |
| Pause      | Esc                   |

---

## 🧠 Project Architecture

* **Engine:** Unity 3D
* **Language:** C#
* **Physics:** Rigidbody-based vehicle system
* **UI:** Unity Canvas
* **Scene Management:** Unity Scene Manager

---

3. Ensure:

   * Rigidbody is attached
   * Colliders are properly configured
   * Input system is mapped (WASD / Arrow keys)

4. Test in Play Mode and adjust:

   * Suspension
   * Torque
   * Steering sensitivity

---

## ⚠️ Known Issues

* Missing assets may cause:

  * Pink materials (shader errors)
  * Invisible terrain or props
  * Broken prefabs

* `RoadArchitect` may require:

  * Road mesh rebuild

* Lighting may appear incorrect until rebaked

---

## 🛠 Troubleshooting

### Pink Materials

* Go to:

  ```
  Edit → Render Pipeline → Upgrade Materials
  ```
* Or manually reassign shaders

---

### Missing Scripts / Broken Prefabs

* Ensure all assets are imported
* Right-click `Assets` → **Reimport All**

---

### Lighting Issues

* Go to:

  ```
  Window → Rendering → Lighting
  ```
* Click **Generate Lighting**

---

## 🚀 Getting Started (Development)

Key areas to modify:

| Component     | Location               |
| ------------- | ---------------------- |
| Vehicle Logic | `VehicleController.cs` |
| Game Flow     | Trigger systems        |
| UI            | Canvas                 |
| Environment   | Scene hierarchy        |

---

## 📁 .gitignore (Recommended)

```gitignore
/Library/
/Temp/
/Obj/
/Build/
*.unitypackage
```

---

## 📌 Notes

* All third-party assets are subject to their original licenses
* Do not redistribute asset files without proper rights
* Use **Git LFS** if large files must be tracked

---

## 🏁 Project Vision

TurboStorm aims to deliver:

* High-speed gameplay
* Realistic driving mechanics
* Expandable architecture

### Planned Features

* Multiplayer mode
* AI traffic system
* Dynamic weather
* Leaderboards

---

## 🤝 Contribution Guidelines

* Pull latest changes before starting work
* Avoid committing large assets
* Follow clean project structure
* Test scenes before pushing

---

## 📬 Support

If the project fails to run:

1. Verify all assets are installed
2. Check Unity version compatibility
3. Reimport the project

