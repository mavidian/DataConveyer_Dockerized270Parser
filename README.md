# DataConveyer_Dockerized270Parser
An ASP.NET Core Web API that uses Data Conveyer to parse an EDI 270 transaction (eligibility inquiry). The web application is published as a Docker image.

The application accepts HTTP POST requests containing an X12 envelope with valid EDI 270 transactions. For example:

```x12
ISA*00*          *00*          *ZZ*123456789012345*ZZ*123456789012346*080503*1705*>*00501*000010216*0*T*:~
GS*HS*1234567890*1234567890*20080503*1705*20213*X*005010X279A1~
ST*270*0001*005010X279A1~
BHT*0022*13*292001*20121127*0753~
HL*1**20*1~
NM1*PR*2*TRICARE*****PI*571132733~
HL*2*1*21*1~
NM1*1P*2*MEMORIAL HOSPITAL*****XX*1044010401~
HL*3*2*22*0~
TRN*1*201211270000000038U*0571132733~
NM1*IL*1*JONES*RAYMOND*S***MI*796347724~
DMG*D8*19800925~
DTP*291*RD8*20121120-20121127~
EQ*47~
III*ZZ*21~
SE*14*0001~
ST*270*1235*005010X279A1~
BHT*0022*13*10001235*20060501*1320~
HL*1**20*1~
NM1*PR*2*ABC COMPANY*****PI*842610001~
HL*2*1*21*1~
NM1*1P*1*JONES*MARCUS****SV*0202034~
HL*3*2*22*1~
NM1*IL*1******MI*11122333307~
HL*4*3*23*0~
TRN*1*93175-012547*9877281234~
NM1*03*1*SMITH*MARY~
DMG*D8*19781014~
DTP*291*D8*20060501~
EQ*30~
SE*15*1235~
GE*2*20213~
IEA*1*000010216~

```

The corresponding response contains extracted transaction elements formatted as JSON:

```json
[
 {
  "ReferenceID": "201211270000000038U",
  "MemberID": "796347724",
  "LastName": "JONES",
  "FirstName": "RAYMOND",
  "DOB": "19800925",
  "ServiceType": "47"
 },
 {
  "ReferenceID": "93175-012547",
  "MemberID": "11122333307",
  "LastName": "SMITH",
  "FirstName": "MARY",
  "DOB": "19781014",
  "ServiceType": "30"
 }
]
```

## Installation

* Fork this repository and clone it onto your local machine, or

* Download this repository onto your local machine.

## Prerequisites

* Docker engine with support for Linux containers, such as [Docker Desktop](https://www.docker.com/products/docker-desktop).  On Windows, [WSL 2 backend](https://docs.docker.com/docker-for-windows/wsl/) is recommended.

* (optional) Client application to submit HTTP POST requests, such as [Postman](https://www.postman.com/).

## Usage

1. Open DataConveyer_Dockerized270Parser solution in Visual Studio.

2. Build and run the application, e.g. hit F5

- *A browser window navigated to `http://localhost:61438/api/parse270` and the content below will show up.**

> Nothing to GET here. Use http POST method passing EDI 270 transaction in the request body.

3. Open command prompt and execute the following `curl` command to send an HTTP POST request.
Note that the My270.x12 file (located in the Data folder) needs to be present in the current location (either copy the file or navigate to the Data folder first).

```cmd
curl -d "@My270.x12" -X POST http://localhost:61438/api/parse270
```

- *Submitted X12 data will get parsed and selected elements extracted into JSON format; the resulting data will be displayed.**

4. As an alternative to step 3. above, start Postman application and submit an HTTP POST request to `http://localhost:61438/api/parse270` with
an X12 envelope (containing EDI 270 transactions, such as depicted above) in the request body. Examine the response contents.

5. (optional) Repeat steps 3 and/or 4 using additional input data.

6. To exit the application, stop the corresponding Docker container. One way to do so it to stop the DataConveyer_Dockerized270Parser container using the Docker Desktop._

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

[Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)

## Copyright

```
Copyright © 2020 Mavidian Technologies Limited Liability Company. All Rights Reserved.
```
