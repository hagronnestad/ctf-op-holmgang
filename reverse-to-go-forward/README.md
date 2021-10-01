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

Started reverse enggineering the `login` method.

```java
    private void login(String str) {
        this.chatAPIInterface.doLogin("Basic " + Base64.getEncoder().encodeToString((getResources().getString(C1316R.string.ricochat_user) + ":" + str).getBytes(StandardCharsets.UTF_8)), userAgent, this.mAndroidId).enqueue(new Callback<Login>() {
            /* class rocks.gojira.ricochat.ChatActivity.C13062 */

    // ... ABBREVIATED
```

Tried to login. `Authorization` header is not correct here, it's just an example.

```bash
curl --location --request GET 'https://gojira.rocks/api/v1/login' \
--header 'User-Agent: RicoChat Client' \
--header 'AndroidId: android_id' \
--header 'Authorization: Basic bGtvOnBhc3N3b3Jk'
```

```c
// TODO: Fill in all the blanks and add files.
```


```bash
curl --location --request GET 'https://gojira.rocks/api/v1/login' \
--header 'User-Agent: RicoChat Client' \
--header 'AndroidId: android_id' \
--header 'Authorization: Basic bGtvOjZDQUY4ODcyMkIyOEJGNTk3QzY5Qzk1MkUxRThFOEQ5'
```

Got a successful login!

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
