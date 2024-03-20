# TODO-List

## Urgent Tasks

Add a `OnExternalGestureCompleted` event to `IGestures` with as parameter an GestureCompletedEventArgs that contains the gesture type and the gesture data.

## Un-ordered List

- [ ] Absolute Position based gestures (Need to start at a specific point)
- [ ] Relative Position based gestures (Can be started from anywhere)

### Node-Based Gestures

- [ ] Gesture Recording
- [ ] Gesture Recognition
    - [ ] Any single touch in a start nodes is a gesture
    - [ ] More than 2 touches is a gesture
- [ ] Manual Gesture Setup
    - [-] Node Types
        - [-] Shared Elements
            - [x] IsGestureStart (bool) (length == 1 || index == 0)
            - [x] IsGestureEnd (bool) (length == 1 || index == length - 1)
            - [x] Position (Vector2)
            - [x] Allowed Position Deviation (double)
            - [x] Timestamp (double)
            - [x] Allowed Timestamp Deviation (double)
            - [x] IsHold (bool)
            - [x] Hold Duration (double)
            - [x] Nodes Can be dragged
            - [ ] Nodes Can be resized (Only for the start and end nodes)
        - [ ] Rectangle
        - [ ] Circle

### Basic Gestures

#### Gestures

- [-] Tap (Any)
    - [x] Relative
    - [x] Absolute
- [ ] Hold (Any)
    - [ ] Relative
    - [ ] Absolute
- [x] Swipe (Single)
    - [x] Relative
    - [x] Absolute
- [ ] Pinch (Single)
    - [ ] Relative
    - [ ] Absolute
- [ ] Rotate (Single)
    - [ ] Relative
    - [ ] Absolute

#### Gestures Unit Tests

- [x] Tap (Any)
    - [x] Relative
    - [x] Absolute
- [ ] Hold (Any)
    - [ ] Relative
    - [ ] Absolute
- [x] Swipe (Single)
    - [x] Relative
    - [x] Absolute
- [ ] Pinch (Single)
    - [ ] Relative
    - [ ] Absolute
- [ ] Rotate (Single)
    - [ ] Relative
    - [ ] Absolute

#### Gestures Setup

- [x] Tap (Any)
    - [x] Relative
    - [x] Absolute

- [ ] Hold (Any)
    - [ ] Relative
    - [ ] Absolute

- [x] Swipe (Single)
    - [x] Relative
    - [x] Absolute

- [ ] Pinch (Single)
    - [ ] Relative
    - [ ] Absolute

- [ ] Rotate (Single)
    - [ ] Relative
    - [ ] Absolute
    
## TODO

- Complete the rest of gestures (functionality + tests + setup)

## HOW TO DETECTED IF TOUCH INPUT IS INSIDE A ROTATED RECTANGLE

- Rotate the rectangle back to 0 degrees
- rotate the input point by the same amount
- check if the rotated point is inside the rectangle