# Health and readiness checks

> health in web app - iwr

docker\04-03-health-readiness-checks\signup-web\Dockerfile

docker-compose -f app/04/web.yml -f app/04/04-03/signup-web.yml config

docker-compose -f app/04/web.yml -f app/04/04-03/signup-web.yml build signup-web

docker-compose -f app/04/web.yml -f app/04/04-03/signup-web.yml up -d

docker ps

docker inspect 04_signup-web_1

> health in api - utility (no curl if Alpine) 

docker\04-03-health-readiness-checks\reference-data-api\Dockerfile

docker-compose -f app/04/web.yml -f app/04/04-03/reference-data-api.yml config

docker-compose -f app/04/web.yml -f app/04/04-03/reference-data-api.yml build reference-data-api

docker-compose -f app/04/web.yml -f app/04/04-03/reference-data-api.yml up -d reference-data-api

docker ps

docker logs 04_reference-data-api_1

> readiness in save handler - container exits if no queue

docker\04-03-health-readiness-checks\save-handler\Dockerfile

docker-compose -f app/04/web.yml -f app/04/04-03/save-handler.yml build signup-save-handler

