# WAL

**WAL** (**W**in**A**PI**L**anguage) is a scripting DSL that compiles to C++. It is intended for executing specific
actions on given time.

## State

Usability: ❌ **in development/very unstable**.

Currently working code (as of 10.12.2022):

```js
fn get_string() {
    return "Hello, world!";  
}
print(get_string());
```

## TODO

- [x] Imports (partially)
- [x] Functions
- [x] Variables (partially)
- [ ] Timed blocks
- [ ] Binary and unary expressions
- [ ] Conditions (if, while, for)
- [ ] STD
-
  - [ ] WinAPI shenanigans (message boxes, window search, ...)
-
  - [ ] SFML(?)
-
  - [ ] Utility (strings, arrays, ...)

## Code example (currently does not compile)

```js
import 'windows';
@defaultUnit(ms)

const WELCOME_STRING = "hello, ";
const MESSAGE_BOX_STRING = "hi!";

fn print_hi() {
	subject = "world!";
	print(WELCOME_STRING + subject);
}

init: {
	print_hi(); // hello, world!
}

1000: message_box(MESSAGE_BOX_STRING, MESSAGE_BOX_STRING);
//or 1000ms: ...
```

## Platform support

This language is intended for use with Windows **only**, as it's STD uses WinAPI quite often. Sorry :(

## Building

Currently the only option to use the compiler is to build it from source.

You'll need:

- .NET 7.0
- CMake
- C++ compiler (MSVC, MinGW, clang)

To use the app.

`⚠ TODO`

## STD

> This DSL has a STD?

Yes! Currently there's only one file (`global.wal`):

```js
import 'cpp/iostream';  
  
fn print(obj) {  
    unsafe { cout << obj << endl; }  
}
```

`unsafe` represents raw C++ code.


