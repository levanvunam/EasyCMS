var removeUnload = removeUnload || false;

window.onbeforeunload = function () {
    if(!removeUnload)
        return 'You have setup the database. If you navigate away from this page without completing setup, all the setup information will be lost. Are you sure you want to reload this page?';
};
