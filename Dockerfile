FROM nginx:stable

ADD ./Builds/misadventure-WebGL /usr/share/nginx/html

EXPOSE 80