<div id="top"></div>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![AGPL License][license-shield]][license-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam">
    <img src="images/selogo2.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">SchoolExam</h3>

  <p align="center">
    project_description
    <br />
    <a href="https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/
schoolexam/issues">Report Bug</a>
    ·
    <a href="https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->

## About The Project

SchoolExam has originated from a design thinking process developed as part of a course about software engineering
with the goal of improving the exam process and exam management at schools.

### Built With

* [C#](https://nextjs.org/)
* [.NET](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [ASP.NET](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-6.0)
* [iText 7 Community](https://itextpdf.com/en/products/itext-7/itext-7-community)
* [MailKit](https://github.com/jstedfast/MailKit)
* [ZXing.Net](https://github.com/micjahn/ZXing.Net)
* [QRCoder](https://github.com/codebude/QRCoder)
* [SkiaSharp](https://github.com/mono/SkiaSharp)
* [AutoMapper](https://automapper.org/)

For testing

* [NUnit](https://nunit.org/)
* [AutoFixture](https://github.com/AutoFixture/AutoFixture)
* [FluentAssertions](https://fluentassertions.com/)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- GETTING STARTED -->

## Getting Started

There are multiple options available for trying out the SchoolExam backend.

### Prerequisites

For running the backend in a local docker environment the following prerequisites are required.

- Install [Docker Engine](https://docs.docker.com/engine/install/#server)
- Install [Docker Compose](https://docs.docker.com/compose/install/)

Alternatively, run the backend without Docker. This requires a few other requirements.

- PostgreSQL database (either local or hosted)
- Install [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

### Installation
For running the backend with Docker, use the following steps.
1. Clone the repo
```sh
git clone https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam.git
```

2. Run docker compose
  ```sh
  docker-compose up
  ```

For running the backend without Docker, the following steps are required.
Before running the backend, the environment variable ```DATABASE_URL``` for the database connection string must be set using the format ```postgres://<user>:<password>@<host>:<port>/school_exam```.
Additionally, there is the possibility to fill the database with dummy data.
This can be achieved by setting the environment variable ```RESTET_DATABASE=1```.

1. Clone the repo
```sh
git clone https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam.git
```
2. Restore packages
  ```sh
  dotnet restore
  ```
3. Build
  ```sh
  dotnet build sources
  ```
4. Run the web service
  ```sh
  dotnet run --project src/SchoolExam.Web/
  ```

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->

## Usage

Serves as a backend for the [Word add-in](https://www.github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam-word-addin) and the [Correction UI](https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam-correction-ui). Therefore
it can be used to locally run those components.

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- ROADMAP -->

## Roadmap

- implement various analytics for individual students to analyze and optimize teaching at schools
- support randomization of tasks
- support exams (writing tasks) that have a grading scheme different from points
- add additional pages dynamically during exams based on demand
- API for review such that points can be corrected based on complaints from students
- store correction overlay independent from scanned PDF
- image recognition for automized correction (multiple choice, matching)

See the [open issues](https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam/issues) for a full list of
proposed features (and known issues).

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTRIBUTING -->

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any
contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also
simply open an issue with the tag "enhancement". Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- LICENSE -->

## License

Distributed under the AGPL License. See `LICENSE.md` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTACT -->

## Contact

Felix Rinderer - rinderer@in.tum.de

Project
Link: [https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam](https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->

## Acknowledgments

* []()#wirfuerschule for the insights on the current situation in schools
* []()Capgemini for the valuable workshops with feedback for the architecture
* []()ISSE chair at the University of Augsburg for giving us room to implement our idea
* []()adesso for providing us a productive collaborative workspace

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[contributors-shield]: https://img.shields.io/github/contributors/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam.svg?style=for-the-badge

[contributors-url]: https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam/graphs/contributors

[forks-shield]: https://img.shields.io/github/forks/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam.svg?style=for-the-badge

[forks-url]: https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam/network/members

[stars-shield]: https://img.shields.io/github/stars/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam.svg?style=for-the-badge

[stars-url]: https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam/stargazers

[issues-shield]: https://img.shields.io/github/issues/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam.svg?style=for-the-badge

[issues-url]: https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam/issues

[license-shield]: https://img.shields.io/github/license/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam.svg?style=for-the-badge

[license-url]: https://github.com/SoftwareEngineering-WS2022-CoCoMonkeys/schoolexam/blob/main/gnu-agpl-v3.0.md

