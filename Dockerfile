FROM ubuntu

RUN ["apt-get", "update", "-y"]

RUN ["apt-get", "install", "-y", "wget"]
RUN ["apt-get", "install", "-y", "gnupg"]
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list'
RUN apt-get update
RUN apt-get install -y google-chrome-stable

# RUN ["apt-get", "install", "-y", "libxss1"]
# RUN ["apt-get", "install", "-y", "libindicator7"]
# RUN ["apt-cache", "search", "libappindicator1"]

# RUN ["apt-get", "install", "-y", "libappindicator1"]
# RUN ["wget", "https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb"]

# RUN ["apt-get", "install", "-f"]
# RUN ["dpkg", "-i", "google-chrome-stable_current_amd64.deb"]

# RUN ["apt", "install", "-y", "./google-chrome-stable_current_amd64.deb"]
RUN ["apt-get", "install", "-y", "dotnet-sdk-8.0"]
RUN ["apt-get", "install", "-y", "packagekit-gtk3-module"]

# Install x11vnc.
# RUN ["apt-get", "install", "-y", "x11vnc"]

# Install xvfb.
# RUN ["apt-get", "install", "-y", "xvfb"]
COPY . /source

WORKDIR /source/src/ConsoleApp

CMD ["dotnet", "run"]
