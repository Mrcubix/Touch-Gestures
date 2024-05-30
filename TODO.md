# TODO-List

## Urgent Tasks

~~Add a `OnExternalGestureCompleted` event to `IGestures` with as parameter an GestureCompletedEventArgs that contains the gesture type and the gesture data.~~

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

### Dev Documentation (Github Wiki)

For each of these topics, indicates in which way such systems could be improved, and what the current limitations are.

- [ ] Architecture Overview
    - [ ] Gesture Handler
    - [ ] Gesture Daemon
    - [ ] Gesture Tiles
    - [ ] Gesture Setup
    - [ ] Gesture Unit Tests

### User Documentation (Github Pages)

- [ ] A download link for the current platform & another to the github releases page

- [x] Installation Guide
    - [x] Dependencies
    - [x] Installation

- [x] Supported Gestures

- [x] Getting Started
    - [x] Binding Overview Screen
    - [x] Adding Gestures
    - [x] Editing Gestures
    - [x] Deleting Gestures

- [ ] Advanced
    - [ ] Node-Based Gestures
    - [ ] Gesture Recording
    - [ ] Gesture Recognition

- [-] FAQ
    - [-] Basic
    - [-] Troubleshooting

### Bindings Rework

- [ ] Wrap different versions of Binding under a single generic interface, with a `Press()` and `Release()` method.
