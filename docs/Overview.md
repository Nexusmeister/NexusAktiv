# AktivCrawler
This page should give an overview about the application(s) and their main intention.

# Structure
The main structure of the software has the following components:
1. Crawler
2. Reading File and analyze contents
3. Save to database for follow-up processes & data analysis (TBD)

## Crawler
The crawler tries to retrieve every match report that is available on the website. If it succeeds in finding a report, the service saves it as a file and publishes a notification for the reader service to start its tasks.

```mermaid
stateDiagram-v2;
    [*] --> Iterating;
    Iterating --> Found;
    Iterating --> NotFound;
    NotFound --> Iterating;
    Found --> SaveFile;
    SaveFile --> Iterating;
    
```