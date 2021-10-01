# Brute force

## Challenge

> - A 7z.zip file was found on a device belonging to Leif Kåre Olsen. The file appears to be password protected.
> - Find the correct password to unpack the file and recover the contents.
> - The flag is a sender email address.


## Cracking

From `Testimony - Kaja Hansen ex girlfriend of Leif Kåre Olsen.docx`:

> She only remembers part of the password, it was the word "Makaroni" and then a character or letter followed by a year.

We can use `john` to crack the password. First get the hash with `zip2john`.

```bash
$ zip2john 7z.zip > 7z.zip.john
ver 2.0 efh 5455 efh 7875 7z.zip/1.gpg PKZIP Encr: 2b chk, TS_chk, cmplen=362, decmplen=398, crc=C9CB000A
```

```bash
$ cat 7z.zip.john
7z.zip/1.gpg:$pkzip2$1*2*2*0*16a*18e*c9cb000a*0*3f*8*16a*c9cb*4f24*2f374b3a8c82ca65e0c2797fa65ce09b18a1d620c6a2c50526bdf6cc7f7578ebd5323e1acac2911b762010deada37b2dcd44f268509e3dc06067176c74abcce39e0cf9fe01238f4b56db2c2c06e3253280e3bcf68dcc6b3b43596440a98dd7619445ad3b6a357e7e541ff9189490b8af8b9394e876e1ea24dd97c2830f59a77463efcdbfa1dd106c4a4bbe037f8b2168c10563b51162619e5abaaeebe0ddc23f9f4f4bd23668239ad912a830c158beb3e3ef636e7b81835e2605ee72b895a676ddd324542366a5441cfc40dbd442a75de149e7dfae5d571dfd6ac3ec31ff737cff5c45518218e2f683d1a66434e63fc018a82456895d9ebc20e00dbbcb2a75218f21a7d9f0337d968f506cdf197401a422a54eeb853c0298544da0af2eef7e49a53baca63eb322b7c9c0ec853dd6837062f582c758a4813550ae912cb52b79e8f643fb75faacefa1656c4430461c863e5a140bf084ab49d2eaeb5b22077f062cfecb85edba5d2346a7d9*$/pkzip2$:1.gpg:7z.zip::7z.zip
```

Crack the hash with the permutations mentioned in the report.

```bash
$ john -mask=Makaroni?a?d?d?d?d 7z.zip.john
Using default input encoding: UTF-8
Loaded 1 password hash (PKZIP [32/64])
Will run 16 OpenMP threads
Press 'q' or Ctrl-C to abort, almost any other key for status
Makaroni<1814    (7z.zip/1.gpg)
1g 0:00:00:00 DONE (2021-09-30 16:01) 25.00g/s 17203Kp/s 17203Kc/s 17203KC/s MakaroniY6765..MakaroniS3924
Use the "--show" option to display all of the cracked passwords reliably
Session completed
```

> Password is: `Makaroni<1814`


```bash
$ unzip 7z.zip
Archive:  7z.zip
[7z.zip] 1.gpg password:
  inflating: 1.gpg
```

```bash
$ file 1.gpg
1.gpg: Zip archive data, at least v2.0 to extract
```

The file `1.gpg` is actually another zip file. Let's extract.

```bash
$ unzip 1.gpg
Archive:  1.gpg
  inflating: mail.txt
```

Let's look inside `mail.txt`

```bash
$ cat mail.txt
From: james.s@protonmail.com
Date: Thursday, September 9, 2021 03:20 PM
To: frank@kripos.no
Subject: Decompiling APK

Hi,

You should take a look at this link regarding APK:
https://stackoverflow.com/questions/21010367/how-to-decompile-an-apk-or-dex-file-on-
android-platform


James

```

## Solution

Flag is: `james.s@protonmail.com`
