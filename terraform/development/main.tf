# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

provider "aws" {
  region  = "eu-west-2"
  version = "~> 2.0"
}
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}
locals {
   parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

data "aws_iam_role" "ec2_container_service_role" {
  name = "ecsServiceRole"
}

data "aws_iam_role" "ecs_task_execution_role" {
  name = "ecsTaskExecutionRole"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-development-apis"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/mosaic-resident-information-api/state"
  }
}

/*    POSTGRES SET UP    */
data "aws_vpc" "development_vpc" {
  tags = {
    Name = "vpc-development-apis-development"
  }
}
data "aws_subnet_ids" "development_private_subnets" {
  vpc_id = data.aws_vpc.development_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}
//database to be used for development purposes, not for DMS
module "postgres_db_development" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
  environment_name = "development"
  vpc_id = data.aws_vpc.development_vpc.id
  db_engine = "postgres"
  db_engine_version = "11.1"
  db_identifier = "mosaic-dev-db"
  db_instance_class = "db.t2.micro"
  db_name = "mosaic_dev"
  db_port  = 5002
  db_username = "${local.parameter_store}/mosaic-api/development/postgres-username"
  db_password = "${local.parameter_store}/mosaic-api/development/postgres-password"
  subnet_ids = data.aws_subnet_ids.development_private_subnets.ids
  db_allocated_storage = 20
  maintenance_window ="sun:10:00-sun:10:30"
  storage_encrypted = false
  multi_az = false //only true if production deployment
  publicly_accessible = false
  project_name = "platform apis"
}
