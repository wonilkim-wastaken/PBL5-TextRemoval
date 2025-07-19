# PBL5-TextRemoval

## Introduction
This project is an implementation of the experiment about correlation between text size and attention.  
This project was created as a school project.

## Requirements
- Unity (2022.3.62f1)
  - [Meta XR All-in-One SDK Asset](https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657)
- Python (3.9)
  - Numpy (1.24.4)
  - OpenCV (4.7.0.72)
  - EasyOCR (1.7.2)

## How to Run
1. Download the project files and place the folder in a location of your choice.  
2. Open [**Unity Hub**](https://unity.com/download).  
3. Go to the **Projects** tab.  
4. Click **Add** → **Add project from disk**, and select the downloaded project folder.  
5. Once the project is added, click on it to launch the **Unity Editor**.  
6. In the Unity Editor, press the **Play** button to start the application.

### Initial setup
Before you run the application for the first time, you need to adjust some values.  
To find following files, see [File Structure](https://github.com/wonilkim-wastaken/PBL5-TextRemoval?tab=readme-ov-file#file-structure) section.  

- **`OCR_init.cs` & `MultiStepTextRemoval.cs`**

  These files contain the Python execution path and script path that must be adjusted before running the project for the first time.  
  Locate the following lines near the top of the file:

  ```csharp
  public string pythonPath; //Absolute path to your Python intepreter
  ```
  Update path to match your own system, for example:
   ```csharp
  public string pythonPath = "C:/Users/USERNAME/AppData/Local/anaconda3/python.exe";
   ```
- **`text_detection.py` & `text_removal_mult.py`**  
  These files contain path to Assets folder that must be adjusted before running the project for the first time.  
  Locate the following line near the top of the file:

  ```python
  # Absolute path to your Assets folder
  base_dir = ""
  ```
  Update path to match your own system, for example:
  ```python
  # Absolute path to your Assets folder
  base_dir = "C:/Users/USERNAME/Desktop/PBL5-TextRemoval/Assets"
  ```

- **Opening the Scene**  
  If you don't see the scene after opening the file for the first time, go to Files -> Open Scene, and select SampleScene2.unity.  
  This file is located under `Assets/Scene` folder

## System Architecture
This system integrates Unity for user interaction and gaze tracking with Python for text detection and text removal.  
There are mainly three stages.

### 1. Text Detection (Python)
- `text_detection.py` script uses *EasyOCR* to detect location and size of text in the scene.
- Detected information is saved as .csv file for later use.

### 2. Text Removal (Python)
- `text_removal_mult.py` script uses OpenCV to generate mask for removal.
- Then, it uses OpenCV inpainting(`cv2.inpaint()`) to remove text from the object.
- Depending on the level (No removal, Partially Removed, Fully Removed), threshold size of the text removal is adjusted.

### 3. Unity Integration
- Inpainted images are saved to disk (`inpaint_out/`) and loaded in real time via `ObjectTextureWatcher.cs`.
- The removal level is toggled using the Slash ( / ) key.
- You can apply the inpainted texture to each object manually by pressing the Right Control (RControl) key.
- The experiment starts with the Space bar, triggering gaze tracking.
- `GazeLogger.cs` records gaze data during the trial.

## File Structure
Key directories inside the `vr_env/Assets` folder:
- **CS_Script/**  
  Contains all C# scripts for Python integration and experiment controls.
  
- **GazeLogs/**  
  Stores exported gaze data logs generated during the experiment (CSV format).
  
- **ImageTextures/**  
  Contains the base images used as stimuli during the experiment.
  
- **inpaint_out/**  
  Output directory for inpainted images with removed text.
> ⚠️ This folder should be empied after each experiment!
  
- **ocr_out/**  
  Contains OCR-processed data including detected text regions and coordinates.
> ⚠️ This folder should be empied after each experiment!
  
- **PythonScripts/**  
  Contains all Python scripts for text detection and image inpainting.
 
## Configuration
### PyRunner
The scene includes a *PyRunner* object, which serves as the interface between Unity and external Python scripts.  
It handles Python execution and texture management through the following scripts:
- **OCR_init (C#)**: Launches `text_detection.py` for initial text detection.
- **MultiStepTextRemoval (C#)**: Launches `text_removal_mult.py` to progressively remove text based on experiment settings.
- **ObjectTextureWatcher (C#)**: Applies texture updates to scene objects when new images are generated.

#### OCR_init
- **Python Path**: Absolute path to your Python interpreter.
- **Script Path**: Path to `text_detection.py` (located in `Assets/PythonScripts`). Adjust according to your file system.
- **Selected Set**: Choose the test set (1 to 3).

#### MultiStepTextRemoval
- **Python Path**: Absolute path to your Python interpreter.
- **Script Path**: Path to `text_removal_mult.py` (in `Assets/PythonScripts`). Adjust according to your file system.
- **Peak Path**: Path to `peak.txt`, which specifies the partial removal height.
- **Selected Set**: Choose the test set (1 to 3).

### The Objects
You can choose the objects that text will be removed.  
For every object in the scene, they have *Object Texture Watcher* script.  
If you disable the script, the object texture will not be changed when the experiment is started.  

#### ObjectTextureWatcher
- **Texture File Name**: Name of the texture image to be used.
- **Folder Path**: Path to the directory containing output textures (e.g., `Assets/inpaint_out`). Adjust according to your file system.

> 💡 You can enable or disable this script on each object to control whether it will be affected during the experiment.

### randomizer.py
This Python script is located in `Assets/ImageTextures` folder.  
You can use this script to randomize the order of flyers.

    python randomizer.py -d [Set Number]

`Set Number` refers to the image set to be randomized.
For example:  
- 1 -> Set 1  
- 2 -> Set 2  
- 3 -> Set 3  

This script also generates random number between 1 to 6.  
This number can be used to determine which image in the selected set will have its text removed during the experiment.

This helps ensure participants are presented with images in a randomized sequence for each session.

## Experimental Environment

The VR scene consists of a simple room where six flyer objects are positioned in a semicircle around the participant's viewpoint.  
Each flyer is a flat image panel placed at eye level, displaying a different visual layout and text.  
The participant remains stationary at the center and can freely look around to view the flyers.

One of the six flyers is randomly selected and modified by removing its text using inpainting.  
This allows the study to compare visual attention patterns across different levels of text visibility in a natural, passive viewing context.  

## Experimental Procedures
When you first run the application, there will be a black screen.  
Black screen will be there until you finish setting up and actually start experiment.

There would be an initialization.  
This initialization process is needed to get text information on the objects.  

After the initialization, you can set the level of removal with **`/(Slash)`** key.  
There are three levels of removal.  

- No removal (level 1)
- Partially removal (level 2)
- Fully removal (level 3)

After you fixed the level of removal, you can apply inpainted texture on the object by pressing **Right Control** key.  
Then, you can start experiment by pressing **space bar** key.  
When the experiment is initiated, black screen will be gone it will record participants' gaze for *eight seconds*.  

After eight seconds, black screen will come back to cover the objects in the scene.  
Also, recorded gaze will be saved in **'GazeLogs'** fodler.

## Author
Name: Wonil Kim  
Email: is0646ep@ed.ritsumei.ac.jp