# Dim grid

## Challenge

> - Reports from the horse show mentions a paper note that the bikers lost.
> - Analyze the paper note, follow the lead and retrieve the tasty solution.
> - The flag is a single word.
> - Example: "Word"

## Note

From `Report - Community survey at Drammen Ridesenter 8-8-21.docx`:

> The same person had also found a crumpled paper note by the place where the two motor cyclists had been standing for a while. The paper note contained a series of numbers and letters that the person was unable to decipher. He was sure that it belonged to one of the two motorcyclists and that it must have fallen out of the pockets of one of them.

From the note:
```
tfypnvvaqdwpbcrcejwfceocwfrivljopghmdpw47hljqrfiqegntxad
```

This looks like an `.onion` URL! Let's browse to it using the Tor Browser.

`http://tfypnvvaqdwpbcrcejwfceocwfrivljopghmdpw47hljqrfiqegntxad.onion/`

We find the following web site:

```html
<!DOCTYPE html>
<html>

    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width,user-scalable=no">
        <title>Knarkern</title>
        <link href="style.css" rel="stylesheet">
    </head>

    <body>
	    <h2>Karkern!!!!</h2>
Betaling med BTC eller XMR, send bestilling til Knarkern_NOR@protonmail.com.

<h2>WEED</h2>
<pre>
Sativa Dominant Hybrid - 60% Sativa / 40% Indica
5g 1000,-
10g 1800,-
</pre>

<h2>HASJ</h2>
<pre>
Sativa hybrid
Billigste hasj på markedet nå!
5g 600,-
10g 1100,-
20g 2200,-
30g 3300,-
50g 5500,-
</pre>

<h2>AMFETAMIN</h2>
<pre>
Hvit og ren speed nesten uten lukt. Ingen meta.
5g 1000,-
10g 1800,-
20g 3000,-
</pre>

<h2>My PeeGeePee Key</h2>
<pre>
-----BEGIN PGP PUBLIC KEY BLOCK-----
Version: GnuPG v2

mQENBGE4brEBCADPVWvM5SqbMHvztaJic8A4l4VWRGOHktDO8N3UtiUubW5+9vOX
N6JFNfFxWNxNtteLAVxXNQuf2uJPrlBQUsWvnvShqTvnkAenmkoNkgLOqLDnapfw
hxf33Ec42Xr9lIsfnKNEj0dAxVqSTZ8ul66c3APDs2rmO/mGP/jXBVmSHRHGHqYp
vB1TvDwOyc0l2jiyP6gCka4zNJTiBSy9OQAHBcm0Az+pQImLxmA6wpt6GrNWio1Y
mHqO/QBswmyWuXmnwoUrl4PHrh53om1qzLCcqe5BOnNt81Ar7WjrFGV75ByAY4RL
aE8theaRE6hDi3z9fjSRlonQ7G9mplWRoNxvABEBAAG0JktuYXJrZXJuIDxLbmFy
a2Vybl9OT1JAcHJvdG9ubWFpbC5jb20+iQE5BBMBCAAjBQJhOG/aAhsPBwsJCAcD
AgEGFQgCCQoLBBYCAwECHgECF4AACgkQrH3/NrGeNE8wHQgAu+oywlLvvJF+XsRk
8+IMuveySw28ewZvzDTf4+flsJqQj6g1InYnjarMc8vVgObSmiA+E5pSBdSWv+mL
glVhqly8LNSk6PyWn2qtm+eZvZ6sQtUw3LvyzA0WmwF6IIz8fTGLKZYdPBYcTN2V
rRLJXMlVGbqBp45J793Ds0apmtvkTohQOPhysJVCSEKCnDb9D+JCvJmQ2FsCpnbu
PzP5oVdI9K1fReUG9Bil5C8oKHGkCOUd5NV5DZ9VAUgntlqy3SHOH99iWH+mFd1V
7vX8pA+hJxaH1r3waA/DzKmCBdDRlY4+gQAnzLaV9twxq/YE3/GHH8ljLKoMNBxD
GSWn2rQcS25hcmtlcm4gPEtuYXJrZXJuQGtuYXJrLm5vPokBOQQTAQgAIwUCYThu
sQIbDwcLCQgHAwIBBhUIAgkKCwQWAgMBAh4BAheAAAoJEKx9/zaxnjRP7hgH+wV+
PqaoxHW45iBwv/aGmr1kSZPAwSnueXJTDlGu8oZGKjdEJVidyj+TIGWQvuStmFL9
oZjEGbdHX5ACdEocpBfILDL8bmxJwVF18pdRy5Pw6aYep2QLN3P+MuSuwvk3IhF7
8pIhvBXUpcYB/+ciqV+Q2i1VIDBwng1sdpYogxHgA2s5RLgBAGKgZA8b2j3By3Xf
oxogLA1jxhHMmgyC4V3vfBqpwTb1vFdVrjLkNK1Zgfbux396ZXzUloSAynm2iHe7
VmdMBUvWKsEjLGgDWONJcsSuTzPnpyw2gZJUVJfBF/lFK84RyNOjelExjAaFCmuK
7UHlYz84HMQueCTh+yG0P0tuYXJrZXJuIChPc3RlcG9wIGVyIGzDuHNuaW5nZW4h
KSA8S25hcmtlcm5fTk9SQHByb3Rvbm1haWwuY29tPokBOQQTAQgAIwUCYUMnQgIb
DwcLCQgHAwIBBhUIAgkKCwQWAgMBAh4BAheAAAoJEKx9/zaxnjRP9R0H/A/TlLvq
bga4hlolY7kjPRXnfZqHPKcnuoHpU0+8H8yrhUjvgerILym61QVfNDGaOGCiR9Cr
Y0ZZAn2aVQdqa+pkdM2u0QVDOTaT6DnddaYR4XVrCcP1MzU77PRZ7+0lvX6Y+R9C
xordPv8CBe6CsX1H1RSCEN6PSa910aR+rE69ksUYLpdcEnCysxwp1xeAmUI4SAIw
iwIN041lFbzZ4wckKOvSYhZjhEqWwT3oulqvVo6OWAJWJ6+Sh841T7fRGFLIQiJU
E8O2yUNhW9AM4gpZBQP29j3hp2PVR+gA7InhoCRlzs9b0Bq7ef9SyOOIJQqiHsGi
JE03JAgPdsMCny0=
=+NQo
-----END PGP PUBLIC KEY BLOCK-----
</pre>
    </body>

</html>
```

I saved the public PGP key to `pgp.public`. Let's inspect it:

```bash
$ gpg pgp.public
gpg: WARNING: no command supplied.  Trying to guess what you mean ...
pub   rsa2048 2021-09-08 [SCE]
      A310DE9FDF0BC5AC6A9B8B70AC7DFF36B19E344F
uid           Knarkern <Knarkern_NOR@protonmail.com>
uid           Knarkern <Knarkern@knark.no>
uid           Knarkern (Ostepop er løsningen!) <Knarkern_NOR@protonmail.com>
```

## Solution

The flag is: `Ostepop`
