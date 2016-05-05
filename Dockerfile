FROM mono:latest

RUN apt-get update && apt-get install -y mono-xsp

COPY . /home

WORKDIR /home

RUN mkdir build

RUN nuget restore -NonInteractive
RUN xbuild /property:Configuration=Release

WORKDIR /home/TweetDis

COPY tweetdis.sh /tweetdis.sh
RUN chmod +x /tweetdis.sh

CMD ["/bin/bash", "-c", "set -e && /tweetdis.sh"]
