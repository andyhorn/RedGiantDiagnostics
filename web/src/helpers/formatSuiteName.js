const formatSuiteName = function(suite) {
    if (suite === null || suite === undefined || suite === "") {
        return "";
    }

    let indexOfSuite = suite.indexOf("suite");
    let suiteName = suite.substring(0, indexOfSuite);
    let suiteTitle = `${suiteName[0].toUpperCase()}${suiteName.substring(1)}`;
    let fullSuiteTitle = `${suiteTitle} Suite`;

    return fullSuiteTitle;
}

export default formatSuiteName;