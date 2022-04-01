---
layout: default
title: Analytics
nav_order: 20
has_children: false
---

# Analytics

## Custom Events

### Track Activities
When player finishes an activity
```
Parameters:
{
    { "myActivity", activityCode },
    { "myActivityResult", result },
};

CustomEvent: myActivity
```

### Track Cards
When player manage a card
```
Parameters:
{
    { "myCardAction", action },
    { "myCardCategory", category },
};

CustomEvent: myCard
```

### Track App
Track specific actions/features, like instant translate
```
Parameters:
{
    { "myAction", action }
};

CustomEvent: myApp
```

### Track Player Score
When player scores
```
Parameters:
{
    { "myScore", score },
    { "myCardAction", action },
};

CustomEvent: myScore
```
