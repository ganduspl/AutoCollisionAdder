# Auto Collision Adder 🛠

Auto Collision Adder is a custom Unity Editor tool that lets you quickly add colliders to multiple objects in your scene.  
It provides flexible filtering options and a live preview of which objects will be affected before applying changes.

---
<img width="373" height="653" alt="image" src="https://github.com/user-attachments/assets/3ea8d01f-185d-4cac-9b45-ef4725953c0e" />


## Features

- Add different types of colliders:
  - Box Collider
  - Mesh Collider
  - Sphere Collider
  - Capsule Collider
- Filters:
  - By object **name** (string contains)
  - By **tag**
  - By **layer**
  - By **3D section box** (interactive box in Scene View)
- Option to **include child objects**
- **Preview** of all objects matching the filters
- **Confirmation dialog** before colliders are applied
- Undo support (`Ctrl+Z`) for added colliders

---

## Installation

1. Clone or download this repository.
2. Copy the `AutoCollisionAdder.cs` file into your Unity project (e.g., inside `Assets/Editor/`).
3. Open Unity and wait for it to compile.

---

## Usage

1. Open the tool from the menu:  
   **`Tools > Auto Collision Adder`**
   <img width="161" height="52" alt="image" src="https://github.com/user-attachments/assets/63928eb0-0530-46a7-8253-6c44f84c4722" />

3. Choose a **Collider Type** from the dropdown.
4. Select the **filters** you want to apply:
   - All Objects (ignores filters)
   - Name Filter – matches objects containing a string
   - Tag Filter – matches objects with a specific tag
   - Layer Filter – matches objects on a given layer
   - Section Filter (3D Box) – use a draggable/resizable box in the Scene View to select objects
5. (Optional) Enable **Include Children** to also affect child objects.
6. Click **"Add Colliders to Objects"**.
7. A confirmation dialog will show how many objects will be affected. Confirm to apply.

---

## Preview

<img width="667" height="238" alt="image" src="https://github.com/user-attachments/assets/73bf509b-f618-45e9-ae1b-54b7c2937632" />


---

## Requirements

- Unity 2021.3 LTS or newer (tested)
- Works with Built-in Render Pipeline, URP, and HDRP

---


## Author

Created by **ganduspl**


## License

This project is licensed under the **MIT License** – feel free to use and modify it in your own projects.
