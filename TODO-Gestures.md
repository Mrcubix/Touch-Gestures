## Un-ordered List

- [x] Absolute Position based gestures (Need to start at a specific point)
- [x] Relative Position based gestures (Can be started from anywhere)

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

- [x] Tap (Any)
    - [x] Relative
    - [x] Absolute

- [x] Hold (Any)
    - [x] Relative
    - [x] Absolute

- [x] Swipe (Single)
    - [x] Relative
    - [x] Absolute

- [x] Pan (Single)
    - [x] Relative
    - [x] Absolute

- [x] Pinch (Single)
    - [x] Relative
    - [x] Absolute

- [x] Rotate (Single)
    - [x] Relative
    - [x] Absolute

#### Gestures Unit Tests

- [x] Tap (Any)
    - [x] Relative
    - [x] Absolute

- [x] Hold (Any)
    - [x] Relative
    - [x] Absolute

- [x] Swipe (Single)
    - [x] Relative
    - [x] Absolute

- [x] Pan (Single)
    - [x] Relative
    - [x] Absolute

- [x] Pinch (Single)
    - [x] Relative
    - [x] Absolute

- [x] Rotate (Single)
    - [x] Relative
    - [x] Absolute

#### Gestures Setup

- [x] Tap (Any)
    - [x] Relative
    - [x] Absolute

- [x] Hold (Any)
    - [x] Relative
    - [x] Absolute

- [x] Swipe (Single)
    - [x] Relative
    - [x] Absolute

- [x] Pan (Single)
    - [x] Relative
    - [x] Absolute

- [x] Pinch (Single)
    - [x] Relative
    - [x] Absolute

- [x] Rotate (Single)
    - [x] Relative
    - [x] Absolute