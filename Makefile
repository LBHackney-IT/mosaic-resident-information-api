.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build mosaic-resident-information-api

.PHONY: serve
serve:
	docker-compose build mosaic-resident-information-api && docker-compose up mosaic-resident-information-api

.PHONY: shell
shell:
	docker-compose run mosaic-resident-information-api bash

.PHONY: test
test:
	docker-compose build mosaic-resident-information-api-test && docker-compose up mosaic-resident-information-api-test

.PHONY: start-test-db
start-test-db:
	docker-compose up -d test-database

.PHONY: restart-test-db
restart-test-db:
	-container_id=$$(eval docker ps -aqf "name=test-database") \
	echo $$container_id \
	-docker kill $$container_id \
	-docker rm $$container_id
	docker-compose up -d test-database

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format
