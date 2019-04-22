const path = require('path');

module.exports = {
    mode: 'development',
    entry: './src/App.tsx',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/    
            },
            {
                test: /\.scss/,
                use: [
                    'style-loader',
                    'css-loader',
                    'sass-loader'
                ],
                exclude: /node_modules/
            }
        ]
    },
    resolve: {
        extensions: [ '.ts', '.tsx', '.js' ]
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'bundle.js'
    },
    devServer: {
        contentBase: path.join(__dirname, 'dist'),
        compress: true,
        port: 3000
    }
}