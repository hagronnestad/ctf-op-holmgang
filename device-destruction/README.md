# Device destruction

## Challenge

> - A partial disk image was recovered from one of the phones in the burnt out car, but there seems to be be a problem reading the data.
> - Solve it and recover the contents of the device. Search the files and find the undercover dog.
> - Find the disk image in the case folder.
> - The flag is the md5 hash of the undercover dog.
> - Example: "a624743321195160f9e2f1f722986106"

## Android Image File

We have access to `recovered_disk_data.img.gz` which is an image file of an Android device. Let's start by gunziping the `.gz`-file.

```bash
gzip -d recovered_disk_data.img.gz
```

What kind of image file are we working with?

```bash
$ binwalk recovered_disk_data.img

DECIMAL       HEXADECIMAL     DESCRIPTION
--------------------------------------------------------------------------------
13631488      0xD00000        Linux EXT filesystem, blocks count: 511744, image size: 524025856, rev 1.0, ext4 filesystem data, UUID=4816042a-efe7-4beb-a35a-ebb3c13dc13d
#... abbreviated
```

Looks like the image contains a Linux filesystem.

### First Try - Lazy approach (Autopsy)

I loaded the image file into Autopsy and carved out all the files it could find. I found an image named `horsedogcow.jpeg`, which I figured was the "undercover dog".

There were multiple versions of `horsedogcow.jpeg` in Autopsy and I tried two MD5-hashes.

- MD5 Hash	6c4c1a76eddbdde27f26cb9757d6689a
- MD5 Hash	b00598f4b4a49c6da9ab044f3ae3ea59

Non of these were correct, must have been an issue with the MD5 hash because the files have been "carved" out or something.

### Second Try - Correct approach - Extract and mount EXT4 file system

```bash
$ binwalk recovered_disk_data.img

DECIMAL       HEXADECIMAL     DESCRIPTION
--------------------------------------------------------------------------------
13631488      0xD00000        Linux EXT filesystem, blocks count: 511744, image size: 524025856, rev 1.0, ext4 filesystem data, UUID=4816042a-efe7-4beb-a35a-ebb3c13dc13d
#... abbreviated
```

Let's run `binwalk -e recovered_disk_data.img` to extract the file system. With the file system extracted we should be able to mount it.

The file system is extracted to `D00000.ext`. Let's mount the file.

```bash
$ mkdir /tmp/ext4/
$ sudo mount -o ro,loop _recovered_disk_data.img.extracted/D00000.ext /tmp/ext4
```

Let's see if we can look inside the file system.

```bash
$ cd /tmp/ext4/
$ ll
total 20
drwxrwxrwx 46 hag  hag   4096 Sep 21 01:08 data
drwx------  2 root root 16384 Sep 21 10:59 lost+found
```

Looks good! Let's browse around...

Inside `data/media/0/Pictures/` we find the "undercover dog" again. Let's check the MD5 hash now.

```bash
$ cd data/media/0/Pictures/

$ md5sum horsedogcow.jpg
16a254ccd660870a1669009da8ecf9b6  horsedogcow.jpg
```

We get a different hash this time!

## Solution

MD5 Hash is: `16a254ccd660870a1669009da8ecf9b6`
