
docker-compose -p test-rabbit --env-file .env -f docker-compose.yml -f docker-compose.override.yml up -d

pause