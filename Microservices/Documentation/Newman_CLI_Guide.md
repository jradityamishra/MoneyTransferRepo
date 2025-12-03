# Newman CLI Testing Guide

## What is Newman?

Newman is a command-line collection runner for Postman. It allows you to run and test Postman Collections directly from the command line.

## Installation

```bash
# Install Newman globally using npm
npm install -g newman

# Install Newman HTML reporter
npm install -g newman-reporter-html
```

## Basic Usage

### Run Collection
```bash
newman run Postman/MoneyTransfer_Microservices.postman_collection.json
```

### Run with Environment
```bash
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json
```

### Generate HTML Report
```bash
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json \
  -r html \
  --reporter-html-export reports/test-report.html
```

### Run with All Options
```bash
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json \
  -r cli,html,json \
  --reporter-html-export reports/test-report.html \
  --reporter-json-export reports/test-report.json \
  --delay-request 500 \
  --timeout-request 10000 \
  --bail
```

## Command Options

### Basic Options
- `-e, --environment` - Specify environment file
- `-g, --globals` - Specify global variables file
- `-d, --iteration-data` - Specify data file for iterations

### Reporters
- `-r, --reporters` - Specify reporters to use (cli, json, html, junit)
- `--reporter-html-export` - Path for HTML report
- `--reporter-json-export` - Path for JSON report
- `--reporter-junit-export` - Path for JUnit XML report

### Execution Options
- `--delay-request` - Delay between requests (milliseconds)
- `--timeout-request` - Request timeout (milliseconds)
- `--bail` - Stop on first test failure
- `-n, --iteration-count` - Number of iterations to run

### Folder/Request Selection
- `--folder` - Run only specific folder
- `--folder "1. Authentication"` - Run only Authentication folder

## Testing Scenarios

### 1. Quick Smoke Test
```bash
# Test only authentication
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json \
  --folder "1. Authentication"
```

### 2. Full Regression Test
```bash
# Run all tests with detailed report
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json \
  -r cli,html,json \
  --reporter-html-export reports/regression-report.html \
  --delay-request 1000
```

### 3. CI/CD Integration
```bash
# Run tests and fail build on errors
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json \
  -r cli,junit \
  --reporter-junit-export reports/junit-report.xml \
  --bail \
  --color off
```

### 4. Load Testing
```bash
# Run 10 iterations
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json \
  -n 10 \
  --delay-request 100 \
  -r cli,json \
  --reporter-json-export reports/load-test.json
```

## Integration Examples

### GitHub Actions Workflow

Create `.github/workflows/api-tests.yml`:

```yaml
name: API Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Install Newman
      run: npm install -g newman newman-reporter-html
    
    - name: Start Services
      run: |
        cd UserMicroservices && dotnet run &
        cd AccountMicroservices && dotnet run &
        cd TransactionMicroservices && dotnet run &
        cd NotificationMicroservices && dotnet run &
        cd OcelotApiGateway && dotnet run &
        sleep 30
    
    - name: Run API Tests
      run: |
        newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
          -e Postman/MoneyTransfer_Development.postman_environment.json \
          -r cli,html \
          --reporter-html-export reports/test-report.html
    
    - name: Upload Test Report
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: test-report
        path: reports/test-report.html
```

### Azure DevOps Pipeline

Create `azure-pipelines.yml`:

```yaml
trigger:
- main
- develop

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '8.0.x'

- task: Npm@1
  inputs:
    command: 'custom'
    customCommand: 'install -g newman newman-reporter-html'

- script: |
    cd UserMicroservices && dotnet run &
    cd AccountMicroservices && dotnet run &
    cd TransactionMicroservices && dotnet run &
    cd NotificationMicroservices && dotnet run &
    cd OcelotApiGateway && dotnet run &
    sleep 30
  displayName: 'Start Services'

- script: |
    newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
      -e Postman/MoneyTransfer_Development.postman_environment.json \
      -r cli,junit \
      --reporter-junit-export reports/junit-report.xml
  displayName: 'Run API Tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'JUnit'
    testResultsFiles: '**/junit-report.xml'
```

### Jenkins Pipeline

Create `Jenkinsfile`:

```groovy
pipeline {
    agent any
    
    stages {
        stage('Setup') {
            steps {
                sh 'npm install -g newman newman-reporter-html'
            }
        }
        
        stage('Start Services') {
            steps {
                sh '''
                    cd UserMicroservices && dotnet run &
                    cd AccountMicroservices && dotnet run &
                    cd TransactionMicroservices && dotnet run &
                    cd NotificationMicroservices && dotnet run &
                    cd OcelotApiGateway && dotnet run &
                    sleep 30
                '''
            }
        }
        
        stage('Run Tests') {
            steps {
                sh '''
                    newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
                      -e Postman/MoneyTransfer_Development.postman_environment.json \
                      -r cli,html,junit \
                      --reporter-html-export reports/test-report.html \
                      --reporter-junit-export reports/junit-report.xml
                '''
            }
        }
    }
    
    post {
        always {
            publishHTML([
                reportDir: 'reports',
                reportFiles: 'test-report.html',
                reportName: 'API Test Report'
            ])
            junit 'reports/junit-report.xml'
        }
    }
}
```

## Docker Integration

### Dockerfile for Newman

```dockerfile
FROM node:18-alpine

# Install Newman
RUN npm install -g newman newman-reporter-html

# Create working directory
WORKDIR /tests

# Copy collection and environment
COPY Postman/ /tests/

# Run tests
CMD ["newman", "run", "MoneyTransfer_Microservices.postman_collection.json", \
     "-e", "MoneyTransfer_Development.postman_environment.json", \
     "-r", "cli,html", \
     "--reporter-html-export", "/reports/test-report.html"]
```

### Docker Compose

```yaml
version: '3.8'

services:
  newman:
    build: .
    volumes:
      - ./reports:/reports
    depends_on:
      - api-gateway
    networks:
      - test-network

  api-gateway:
    image: your-gateway-image
    ports:
      - "7000:7000"
    networks:
      - test-network

networks:
  test-network:
    driver: bridge
```

## Batch Scripts

### Windows - run-tests.bat

```batch
@echo off
echo Starting API Tests...

newman run Postman\MoneyTransfer_Microservices.postman_collection.json ^
  -e Postman\MoneyTransfer_Development.postman_environment.json ^
  -r cli,html ^
  --reporter-html-export reports\test-report-%date:~-4,4%%date:~-10,2%%date:~-7,2%-%time:~0,2%%time:~3,2%%time:~6,2%.html ^
  --delay-request 500

if %errorlevel% neq 0 (
    echo Tests failed!
    exit /b %errorlevel%
)

echo Tests completed successfully!
```

### Linux/Mac - run-tests.sh

```bash
#!/bin/bash

echo "Starting API Tests..."

newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json \
  -r cli,html \
  --reporter-html-export reports/test-report-$(date +%Y%m%d-%H%M%S).html \
  --delay-request 500

if [ $? -ne 0 ]; then
    echo "Tests failed!"
    exit 1
fi

echo "Tests completed successfully!"
```

## Report Examples

### CLI Output
```
?????????????????????????????????????????????????
?                         ? executed ?   failed ?
?????????????????????????????????????????????????
?              iterations ?        1 ?        0 ?
?????????????????????????????????????????????????
?                requests ?       35 ?        0 ?
?????????????????????????????????????????????????
?            test-scripts ?       70 ?        0 ?
?????????????????????????????????????????????????
?      prerequest-scripts ?       35 ?        0 ?
?????????????????????????????????????????????????
?              assertions ?      105 ?        0 ?
?????????????????????????????????????????????????
? total run duration: 45.6s                      ?
??????????????????????????????????????????????????
? total data received: 12.4KB (approx)           ?
??????????????????????????????????????????????????
? average response time: 1.2s                    ?
??????????????????????????????????????????????????
```

## Troubleshooting

### Issue: "Collection not found"
```bash
# Use absolute path
newman run /full/path/to/collection.json
```

### Issue: "Environment variables not working"
```bash
# Verify environment file path
newman run collection.json -e /correct/path/to/environment.json
```

### Issue: "Connection refused"
```bash
# Ensure services are running
# Add delay before running tests
sleep 30 && newman run collection.json
```

### Issue: "Tests timing out"
```bash
# Increase timeout
newman run collection.json --timeout-request 30000
```

## Best Practices

1. **Use Delays**: Add delay between requests to avoid overwhelming the server
2. **Set Timeouts**: Configure appropriate timeouts for your environment
3. **Generate Reports**: Always generate HTML/JSON reports for analysis
4. **Version Control**: Store collections and environments in version control
5. **CI/CD Integration**: Automate tests in your deployment pipeline
6. **Folder Execution**: Run specific test folders during development
7. **Iterations**: Use iterations for load testing
8. **Bail Option**: Use --bail in CI/CD to stop on first failure

## Advanced Usage

### Data-Driven Testing

Create `test-data.json`:
```json
[
  {
    "username": "user1",
    "email": "user1@example.com",
    "amount": 100
  },
  {
    "username": "user2",
    "email": "user2@example.com",
    "amount": 200
  }
]
```

Run with data:
```bash
newman run collection.json \
  -e environment.json \
  -d test-data.json
```

### Custom Reports with Template

```bash
newman run collection.json \
  -e environment.json \
  -r html \
  --reporter-html-template custom-template.hbs \
  --reporter-html-export custom-report.html
```

## Resources

- Newman Documentation: https://learning.postman.com/docs/running-collections/using-newman-cli/
- Newman GitHub: https://github.com/postmanlabs/newman
- Postman Learning Center: https://learning.postman.com/

---

**Ready to Automate?** ??

Run your first automated test:
```bash
newman run Postman/MoneyTransfer_Microservices.postman_collection.json \
  -e Postman/MoneyTransfer_Development.postman_environment.json
```
