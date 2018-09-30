const path = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");
var webpack = require("webpack");
var stylesCss = new ExtractTextPlugin("[name].css");

var options = {
    minimize: false
};

module.exports = {
  mode: 'development',
  entry: {
      styles: './styles.scss',
      scripts: './scripts.js'
  },
  output: {
    path: path.resolve(__dirname, '..', 'Resume', 'Resources', 'wwwroot', 'dist')
  },
  module: {
    rules: [
      {
        test: /\.scss/,
        use: stylesCss.extract({
          fallback: "style-loader",
          use: [
            {
              loader: 'css-loader',
              options: options
            },
            'sass-loader',
            'resolve-url-loader'
          ]
        })
      },
      {
        test: /\.css$/,
        use: stylesCss.extract({
          fallback: "style-loader",
          use: [
            {
              loader: 'css-loader',
              options: options
            }
          ]
        })
      },
      {
        test: /\.(svg|woff|woff2|ttf|eot|jpg|gif|png)$/,
        use: [
          {
            loader: 'file-loader'
          }
        ]
      }
    ]
  },
  plugins: [
      stylesCss,
      new webpack.ProvidePlugin({
          $: "jquery",
          jQuery: "jquery"
      })
  ]
};
