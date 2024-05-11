#!usr/bin/env
#has to be added in directory /home/pavixon/automation
#crontab -u pavixon -e
#* * * * * /home/pavixon/automation/deploy-simpleapi.sh
#sudo nano filename can open text editor

export DOCKER_CONTENT=pavixon

LOCK_FILE="${pwd}/simple-api.lock"
cd /home/pavixon/services/Self-hosted-simple-api
flock -n $LOCK_FILE ./scripts/deploy-if-changed.sh >> /home/pavixon/automation/deploy-simpleapi.log 2>&1 