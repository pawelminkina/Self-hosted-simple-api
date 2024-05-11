#!/usr/bin/env bash
#has to be added in directory /home/pavixon/automation with chmod +x
#crontab -u pavixon -e
#* * * * * /home/pavixon/automation/deploy-simpleapi.sh
#sudo nano filename can open text editor. Some change to trigger deployment

export DOCKER_CONTENT=pavixon

LOCK_FILE="${pwd}/simple-api.lock"
cd /home/pavixon/services/Self-hosted-simple-api
flock -n $LOCK_FILE ./scripts/bash/deploy-if-changed.sh >> /home/pavixon/automation/deploy-simpleapi.log 2>&1 