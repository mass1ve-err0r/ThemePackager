# ThemePackager
(Re-)Package iOS Themes on Windows.

![showcaseX](/assets/showcase.png)


### Introduction
It's a simple C# application I wrote to repackage iOS Themes on Windows (yes, it'll create a full iOS-compatible debian package).

It doesn't really have a purpose outside of repackaging, I created it because I got tired of pushing my created Themes to my iPhone and having to type down the commands there to repackage.

This doesn't rely on WSL, just extract and you're good to go. For technical details/ how this works, scroll down to *INTERNALS*

**PLEASE READ THROUGH ALL OF THE INSTALLATION STEPS & FAQ BEFORE REPORTING A BUG**

---

### Features
- Create full iOS-compatible .deb
- Customize the control file properties
- Decent UI lmao

---

### Requirements
- .NET Framework 4.7.2
- Windows 10

---

### How-To Install
It's pretty straightforward:
1. Get the latest release from [HERE](https://github.com/mass1ve-err0r/ThemePackager/releases) & extract the ZIP to any location of your choice (f.e. Desktop). The password is "m1e0"
2. Run ThemePackager
3. Set *every* field (IconBundles, Bundles & Info.plist location, control file properties)
4. Click on "Create Package" and wait until it's done.
5. Your finished deb will be located in the \_output folder

From here it's ready to be distributed & installed on jailbroken i-Devices.

---

### FAQ
- *It doesn't work?!*
  - Make sure you meet the requirements & set every field before attempting to create a package
- *Why do I need Bundles/ Info.plist ?*
  - You don-'t necessarily, you can just point it to an empty folder & empty file, however that implies your Theme is incomplete
- *The output/ .deb file is a few KB, is that normal?*
  - Either yes due to the size of your theme OR it malfunctioned in which case, contact me and report the bug with details and your Windows Version

---

### Internals
Some of you might be interested in how exactly this works since you cannot create debian *archives* on Windows. The clue is in the italic word.

A .deb is nothing more than a renamed `ar` archive containing three files:
- A `debian-binary` which is a plaintext with contents "2.0\n" (UNIX -LF)
- A tarball named `control.tar` which contains the control file
- A tarball named `data.tar` which contains the contents

So the main goal at first was to find a way to create an `ar` archive on Windows, since tarballs are a trivial task for 7zip. After some searching I figured to check for ported GNU binutils and voila! From binutils I stripped the `ar.exe` and included it in my project, that's part of the magic.


---

### Credits / Thanks
- The Debian Project for the docs
- The GNU Project for binutils

---

### License
It's under GPL v3. In case you use something from here, please do credit.
