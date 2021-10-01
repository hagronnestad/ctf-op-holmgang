# Reverse to go forward

## Challenge

> - Digital forensic investigations of a smartphone device used by Leif Kåre Olsen uncovered a suspicious app, which must be reversed to find out what it does.
> - Investigators suspect it may be a gateway to a secure chat service that may contain content that could be crucial for the investigation.
> - Find the APK for the app in the case folder.
> - Access the chat service and retrieve the secret inside the messages.
> - The flag is a hash.
> - Example: "6e50e17d8234e768021cd39b344a1031"


## Decompilation

Decompiled `ricocalc.apk` using `jadx-gui`. Found interesting strings in `strings.xml`.

Keys from `strings.xml`:
```xml
    <string name="ricochat_chat">RicoChat</string>
    <string name="ricochat_key">3FE58FE3DD35BBA4CD63D62E4FFFD8D2</string>
    <string name="ricochat_room">51e77454-4bd8-48b7-8a75-5b5b09807320</string>
    <string name="ricochat_server">https://gojira.rocks/</string>
    <string name="ricochat_user">lko</string>
```

Found the `User-Agent` used by the API client:
```java
    private static final String userAgent = "RicoChat Client";
```

Found the API interface:

```java
public interface ChatAPIInterface {
    @GET("/api/v1/rooms/{roomId}")
    Call<RoomMessages> doGetRoomMessages(@Path("roomId") String str, @Header("Authorization") String str2, @Header("User-Agent") String str3, @Header("AndroidId") String str4);

    @GET("/api/v1/user/{userId}")
    Call<UserDetails> doGetUserDetails(@Path("userId") String str, @Header("Authorization") String str2, @Header("User-Agent") String str3, @Header("AndroidId") String str4);

    @GET("/api/v1/login")
    Call<Login> doLogin(@Header("Authorization") String str, @Header("User-Agent") String str2, @Header("AndroidId") String str3);

    @POST("/api/v1/rooms/{roomId}/message")
    Call<UserDetails> doPostMessage(@Path("roomId") String str, @Header("Authorization") String str2, @Header("User-Agent") String str3, @Header("AndroidId") String str4, @Field("body") String str5, @Field("userId") String str6);
}
```

Started reverse engineering the `login` method.

```java
private void login(String str) {
    this.chatAPIInterface.doLogin("Basic " + Base64.getEncoder().encodeToString((getResources().getString(C1316R.string.ricochat_user) + ":" + str).getBytes(StandardCharsets.UTF_8)), userAgent, this.mAndroidId).enqueue(new Callback<Login>() {
        /* class rocks.gojira.ricochat.ChatActivity.C13062 */

// ... ABBREVIATED
```

This code AES encrypts `mResultText.getText()` and compares it to the `C1316R.string.ricochat_key` and a regex pattern. If they match the Chat activity is started with `CHAT_TOKEN` set to the cipher text of `mFormulaText.getText()` inside the bundle.

```java
String str = "" + ((Object) this.mFormulaText.getText());
Log.i("Calculator", str);
if (KeyUtils.m48a("" + ((Object) this.mResultText.getText())).equals(getResources().getString(C1316R.string.ricochat_key)) && Pattern.compile("[−+÷×0-9costansin()]{3,6}").matcher(str).matches()) {
    Intent intent = new Intent(this, ChatActivity.class);
    intent.putExtra(CHAT_TOKEN, KeyUtils.m48a(str));
    startActivity(intent);
}
```

This means that we need to find the correct result AND the correct formula.

First I misread the code and thought that the cipher text we needed to find was based on the formula only.

The regex pattern check is the important part here, because it narrows down the formula for us. We know that the formula can only contain the following characters: `−+÷×0-9costansin()` and if I understand Java RegEx correctly, the formula needs to be 3-6 characters long.

As I was completely focused on the formula at first, I started to generate every possible combination of formulas based on the allowed characters.

To do this I put together and used the `Formula Permutations.linq` LINQPad script and generated a file for each combinations with length 3-6.

```
$ ll | grep allcom
-rwxrwxrwx 1 hag hag      85316 Sep 30 19:38 allcombinations3.txt
-rwxrwxrwx 1 hag hag    2592892 Sep 30 19:39 allcombinations4.txt
-rwxrwxrwx 1 hag hag  152338432 Sep 30 22:41 allcombinations5.txt
-rwxrwxrwx 1 hag hag 2187497472 Sep 30 19:39 allcombinations6.txt
```

`allcombinations6.txt` is HUGE (>2GB)! Only `allcombinations3.txt` is included in the repo for demonstrations purposes.

At this point I converted the `KeyUtils` class from Java to C# and I tested the `KeyUtils`-class in online Java runners to make sure that cipertexts produced was the same.

Then I started to bruteforce all the formula permutations against the key in the app, but as mentioned, the formula is never checked against the key in the app, only the result is.

Then I finally realized that I've misread the code and needed to find the result that matched the key, not the formula.

Not realizing I could just decrypt the key in the app to find the expected result, I started to bruteforce the result. Look at `BruteforceResult()` in the AESSolver-project. Anyway; I did get a match this time; `42`.

Output from AESSolver:
```
Result MATCH! N: 42, AES: 3FE58FE3DD35BBA4CD63D62E4FFFD8D2
```

I finally got past the Calculator activity and into the Chat activity, but I still needed to find the expected formula, because that is the key needed to log into the API.

I could use my prebuilt formula permutations from before and use them as input to an expression solver and find all the formulas that results in `42`, then I could bruteforce all the matcing formulas.

However, I was a bit desperate at this point at just started to type in formulas in the app that would result in `42`.

I tried:
- 21+21
- 84÷2
- 168÷4

I got lucky! `168÷4` was the correct formula, the app logged me in.

That formula can be found in `allcombinations5.txt`, so my initial thought would also work.

I used `Console.WriteLine(AesEncrypt("168÷4"));` which gives the cipher text `6CAF88722B28BF597C69C952E1E8E8D9`.

I entered that into Postman as shown below:

![](001-login.png "")

"Basic Auth" is just `base64({user}:{pass})`:

```bash
$ echo -n "lko:6CAF88722B28BF597C69C952E1E8E8D9" | base64
bGtvOjZDQUY4ODcyMkIyOEJGNTk3QzY5Qzk1MkUxRThFOEQ5
```

The request from Postman as a `curl`-command:

```bash
curl --location --request GET 'https://gojira.rocks/api/v1/login' \
--header 'User-Agent: RicoChat Client' \
--header 'AndroidId: android_id' \
--header 'Authorization: Basic bGtvOjZDQUY4ODcyMkIyOEJGNTk3QzY5Qzk1MkUxRThFOEQ5'
```

I got a successful login!

Result:
```json
{
    "data": {
        "subscription": {
            "message": "Your RicoChat subscription has expired. Please contact your salesman!",
            "status": false
        },
        "token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwdWJsaWNfaWQiOiJmMTM2NmI3My0yNDI5LTQ3OTYtYTFhZi02NjhiNjgzYWYyOGQifQ.jXM24bV_PKTxUe3jUDzALNl-PCoSF8Frv-BulNtVdxw",
        "user": {
            "email": "leif_k_olsen@example.com",
            "name": "Leif KO",
            "public_id": "f1366b73-2429-4796-a1af-668b683af28d",
            "username": "lko"
        }
    },
    "message": "Login successful",
    "meta": {
        "api": "/api/v1",
        "server": "ricochat 0.1.6"
    },
    "status": true
}
```

Let's try to retrieve the messages using the returned `token` and the room id from `ricochat_room` above:

```bash
curl --location --request GET 'https://gojira.rocks/api/v1/rooms/51e77454-4bd8-48b7-8a75-5b5b09807320' \
--header 'User-Agent: RicoChat Client' \
--header 'AndroidId: android_id' \
--header 'Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwdWJsaWNfaWQiOiJmMTM2NmI3My0yNDI5LTQ3OTYtYTFhZi02NjhiNjgzYWYyOGQifQ.jXM24bV_PKTxUe3jUDzALNl-PCoSF8Frv-BulNtVdxw'
```

Result:

```json
{
    "data": {
        "room": {
            "messages": [
                {
                    "body": "Hei Helene :-) Tester RicoChat appen. Fungerer dette?",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-01 17:07:01"
                },
                {
                    "body": "Hei elskling!❤️ Det fungerer!!1! Hadde litt problms m å finne riktig regnestykke, hehe 😝",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-01 17:11:02"
                },
                {
                    "body": "😊😊😊 Kult med kalkis, ass. Har alltid vært glad i matte, NOT! Fikk tips fra Harry om denne appen, han bruker det når han selger tjall",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-01 17:12:03"
                },
                {
                    "body": "Funker utrolig bra! Du er så flink med sånne tekniske ting altså ❤️  Nå kan vi chatte uten å være redd for at noen finner ut om oss❤️ ❤️ ❤️ ",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-01 17:13:04"
                },
                {
                    "body": "Ja, endelig! Glad i deg, nussetrollet!❤️  Ok, må stikke, HH ruser sykkelen i bakgården her",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-01 17:14:05"
                },
                {
                    "body": "❤️ xxx❤️ ",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-01-05 17:15:06"
                },
                {
                    "body": "Hei! Det er jo bursdagen din snart... ikke sant? ❤️  Har tenkt på en presang...",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:12:01"
                },
                {
                    "body": "Har du??? OMG, du den mest følsomme biker-duden jeg kjenner ❤️ ❤️ ❤️ ",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-05 19:13:03"
                },
                {
                    "body": "Trenger litt hjelp av deg for å få det til, er du med på en vill ide?",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:15:04"
                },
                {
                    "body": "Anything for you, darling!1! ❤️ ",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-05 19:17:06"
                },
                {
                    "body": "Du har jo gått lenge og sikla på den drømmehesten din, Lucky Scout, ikke sant? Men faren din vil ikke kjøpe den til deg? Jeg vet hvordan vi kan få det til...",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:20:08"
                },
                {
                    "body": "OMG!! Ponies!1!!! ❤️ 😊❤️  Mener du det???  Nå blir jeg blir helt svimmel, den har jeg alltid ønsket meg! Pappa er så gnien, han vil ikke kjøpe en til meg! Han har masse penger men svir dem dem bare av på drivstoff til den teite cabincruiseren sin",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-05 19:22:09"
                },
                {
                    "body": "Ja, mener det helt seriøst! Har et bombesikkert opplegg på gang! 👍 Har funnet en måte vi kan få faren din til å betale for den, det er til pass for han det rasshølet. Han burde behandle deg bedre, du som er så søt",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:25:10"
                },
                {
                    "body": "Åh...du sier så fine ting, Leifegull!! ❤️ jeg er med på alt, det vet du!",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-05 19:26:12"
                },
                {
                    "body": "Ok, her er planen: Du skal på stevne snart, ikke sant? Jeg og HH kan ordne en liksom-kidnapping av deg på vei til stevnet, og så drar vi til hytta til Aksel på Tjøme og feirer!",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:28:13"
                },
                {
                    "body": "Så sender vi en ransom-note til den rike faren din! Han får helt sjokk, og kommer til å punge ut med EN gang!",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:29:15"
                },
                {
                    "body": "Har fått hjelp av en kompis til å ordne en sånn BitCoin adresse så vi kan få pengene på, det er UMULIG å spore! Easy peasy! Har allerede satt opp en konto på BitCoinStockTrader. Brukte en sånn super secret passord hash som er umulig å gjette: cff875e42b0d4c42c2b53ada9fca0dd8",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:32:17"
                },
                {
                    "body": "OMG, leifegull! ❤️ Visste ikke at du var en sånn mesterhjerne!1! dette høres helt vanntett ut!!! Så romantisk å bli kidnappet! 🥰Kan vi bli igjen på hytta etterpå...? Har  kjøpt ny neglisje...❤️ ",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-05 19:35:19"
                },
                {
                    "body": "Neglisje? Ohh, den har jeg lyst å prøve!",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:37:20"
                },
                {
                    "body": "haha, tøysegutten🥰🥰",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-05 19:38:22"
                },
                {
                    "body": "Hehe, elsker deg! Da legger jeg og HH planer og så snakkes vi ...",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-05 19:39:25"
                },
                {
                    "body": "Spennende!!!! Love U xxx ❤️ ",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-05 19:40:27"
                },
                {
                    "body": "Er du klar? I dag skjer det!",
                    "from": "f1366b73-2429-4796-a1af-668b683af28d",
                    "posted": "2021-08-12 08:05:01"
                },
                {
                    "body": "Jeg er klar! ❤️  be gentle...❤️ ",
                    "from": "acc83095-2df0-4a18-89c6-0f0296574827",
                    "posted": "2021-08-12 08:06:04"
                }
            ],
            "name": "Lovebirds nest"
        }
    },
    "message": "Room fetched successfully",
    "meta": {
        "api": "/api/v1",
        "server": "ricochat 0.1.6"
    },
    "status": true
}
```


## Solution

Hash is: `cff875e42b0d4c42c2b53ada9fca0dd8`
