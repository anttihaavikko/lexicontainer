mergeInto(LibraryManager.library, {
    PromptName: function (callback) {
        if (navigator.maxTouchPoints > 0) {
            const name = prompt('Please enter your name', '');
            if (name) {
                const trimmed = name.substring(0, 20).replace(/[^0-9a-z- ]/gi, '');
                var buffer = _malloc(lengthBytesUTF8(trimmed) + 1);
                writeStringToMemory(trimmed, buffer);
                {{{ makeDynCall('vi', 'callback') }}} (buffer);
            }
        }
    },
});