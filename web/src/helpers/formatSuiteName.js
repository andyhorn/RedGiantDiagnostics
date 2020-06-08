const formatSuiteName = function(suite) {
  if (suite === null || suite === undefined || suite === "") {
    return "";
  }

  let suiteName = suite;
  let includesSuite = false;
  if (suite.includes("suite")) {
    let indexOfSuite = suite.indexOf("suite");
    suiteName = suite.substring(0, indexOfSuite);
    includesSuite = true;
  }

  let suiteTitle = `${suiteName[0].toUpperCase()}${suiteName.substring(1)}`;

  let fullSuiteTitle = includesSuite ? `${suiteTitle} Suite` : suiteTitle;

  return fullSuiteTitle;
};

export default formatSuiteName;
