# build a folder that can be uploaded as a release

import os
import shutil
import zipfile

if os.path.exists("build"):
    shutil.rmtree("build")

os.makedirs("build")
print("Rebuilt build folder")


arch = os.environ.get("PROCESSOR_ARCHITECTURE", "")
arch_w6432 = os.environ.get("PROCESSOR_ARCHITEW6432", "")

if arch == "AMD64" or arch_w6432 == "AMD64":
    arch = "x64"
else:
    arch = "x86"
print(f"Detected Windows architecture: {arch}")

if not os.path.isfile(f"extra/BepInEx/BepInEx_win_{arch}_5.4.23.3.zip"):
    raise FileNotFoundError(f"Required zip file not found: extra/BepInEx/BepInEx_win_{arch}_5.4.23.3.zip")

with zipfile.ZipFile(f"extra/BepInEx/BepInEx_win_{arch}_5.4.23.3.zip", 'r') as zip_ref:
    zip_ref.extractall("./build/MedalPeakPlugin")

print(f"Extracted extra/BepInEx/BepInEx_win_{arch}_5.4.23.3.zip")


# this is going to throw a warning, but that doesn't matter since we clean up the output folder beforehand
os.system("dotnet build ./MedalPeakPlugin --output ./build/MedalPeakPlugin/BepInEx/plugins/MedalTV-MedalPeakPlugin")
print("Built DLL")

shutil.copy2("build/MedalPeakPlugin/BepInEx/plugins/MedalTV-MedalPeakPlugin/MedalPeakPlugin.dll", "build/MedalPeakPlugin.dll")
print("Replicated DLL for 2nd-method installation")


with zipfile.ZipFile("build/MedalPeakPlugin.zip", 'w', zipfile.ZIP_DEFLATED) as zipf:
    for root, dirs, files in os.walk("build/MedalPeakPlugin"):
        for file in files:
            file_path = os.path.join(root, file)
            # Arcname ensures correct folder structure inside the zip
            arcname = os.path.relpath(file_path, start="build/MedalPeakPlugin")
            zipf.write(file_path, arcname)
print("Created zip archive from build")

shutil.rmtree("build/MedalPeakPlugin")
print("Deleted folder that was replaced by zip")

print("Finished build for release")