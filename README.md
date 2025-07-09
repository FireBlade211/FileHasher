# FileHasher
A WinUI 3 application for hashing text with different algorithms.

# Table of Contents
[View full](README.md#full-table-of-contents)
- [Installation](README.md#installation)
- - [Certificate error (0x800B010A)](README.md#certificate-error-0x800b010a)
- [Dependencies](README.md#dependencies)
- [License](README.md#license)
- [Contributions](README.md#contributions)
- [How to Use](README.md#how-to-use)  
- - [Hash Page](README.md#hash-page)
- - [Settings](README.md#settings)

# Installation
Download the latest release from the [Releases](https://github.com/FireBlade211/FileHasher/releases) page, download the **ZIP file** for your architecture (e.g, "FileHasher_1.0_x64.zip"), unzip it, and run the **MSIX** file (e.g, "FileHasher_1.0.0.0_x64.msix").

## Certificate error (*0x800B010A*)
If you get a certificate error (**0x800B010A**) after running the **MSIX** file, **close out of the installer** and follow the steps below.
1. Go back to the folder you unzipped.
2. Find the *.cer* file and double-click on it.
3. In the **Certificate** dialog, click **Install Certificate**.
4. In the **Certificate Import Wizard**, select *Local Machine* and press Next.
5. Authenticate with **User Account Control** to continue.
6. On the next page, select **Place all certificates in the following store**, and in the *Certificate store* box, click **Browse**, find **Trusted Root Certification Authorities**, and press OK. Then, press Next.
7. Finally, hit Finish, close the message box that shows up, and close out of the **Certificate** dialog.
8. Now, you can run the **MSIX** file again, and, if everything was done correctly, you should be able to install the program.

# Dependencies
This app depends on the **Windows App SDK**, but modern versions of Windows should include it by default.

# License
**FileHasher** is open-source and available under the **MIT License**. See the [LICENSE](LICENSE) file for more details.

# Contributions
**FileHasher** is free and open-source. Contributions are welcome!

# How to Use
After running **FileHasher**, you'll see large text saying to pick a hash algorithm from the navigation bar to begin. You can click the **expand** button at the top to view it in full. Once you pick an algorithm, you'll be taken to the hash page:

## Hash Page
In this page, you can type any source text in the **Source** box to view its hash in the **Hash** box for the algorithm you selected. At the top, you can open a file or press the **More** button to toggle live update. If live update is turned *off* (default), then the **Generate Hash** button will appear, that will generate the hash when pressed. If not, the hash will be automatically generated as you type. You can press the **Copy** button to copy the generated hash.

## Settings
This tab allows you to configure app settings, which include:

### Appearance & behavior
- **App theme** - Choose the theme that should be used by **FileHasher**. *Use system theme* will follow your Windows theme.
- **Navigation style** - Choose a navigation style to use. Options: **Left, Left (overlay), Left (compact), Top**. The default is **Top**.
#### Sound
Enable or disable sound effects.
- **Volume** - Configure the volume of the sound effects.
- **Enable Spatial Audio** - Enable or disable spatial audio for a more immersive experience.
- *Test audio button* - As the text on the button says, you can click on this button to play a random sound effect.
----
- **Window background style** - Pick a window background style. **Acrylic (thin)** looks the nicest, but it also may be glitchy, so use at your own risk.
### About
View information about **FileHasher**, such as your version. This will automatically show a special card on **Debug** builds.

# Full Table of Contents
[Go to Top](README.md#filehasher)
[View compact](README.md#table-of-contents)
- [Installation](README.md#installation)
- - [Certificate error (0x800B010A)](README.md#certificate-error-0x800b010a)
- [Dependencies](README.md#dependencies)
- [License](README.md#license)
- [Contributions](README.md#contributions)
- [How to Use](README.md#how-to-use)
- - [Hash Page](README.md#hash-page)
- - [Settings](README.md#settings)
- - - [Appearance & behavior](README.md#appearance--behavior)
- - - - [Sound](README.md#sound)
- - - [About](README.md#about)
