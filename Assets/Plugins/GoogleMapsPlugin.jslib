mergeInto(LibraryManager.library, {
    OpenGoogleMaps: function(addressPtr) {
        var address = UTF8ToString(addressPtr);
        var url = "https://www.google.com/maps/search/?api=1&query=" + encodeURIComponent(address);
        window.open(url, "_blank");
    }
});