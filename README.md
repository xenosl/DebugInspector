# DebugInspector

Debug Inspector is an enhanced unity object inspector for debug purpose.
See Assets/ShuHai/DebugInspector Manual.pdf for usage information.

NOTE: __This project is not supported start from Unity-2018.3.__

Reason: System.Reflection.Emit doesn't exist any more and there are lots of dynamically generated code for performance optimaization.

## Features

- List all members of selected object and compoents of GameObject recusively in a hierarchy tree.
- Change field and property values directly on GUI.
- Auto split collection itmes into pages.
- Collect error logs and exception when accessing properties.
- List stack trace of error logs and exception when accessing properties and open source file with a simple click.
- Filter display objects by search text and filter options.