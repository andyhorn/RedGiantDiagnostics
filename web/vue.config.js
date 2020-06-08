module.exports = {
  chainWebpack: config => {
    config.plugin("html").tap(args => {
      args[0].title = "Red Giant Diagnostics";
      return args;
    });
  }
};
