import * as React from 'react';

export default class ColumnTitle extends React.Component<{ name: string }> {
    public render() {
        return <h2 style={styleColumnTitleText}><strong>{this.props.name}</strong></h2>
    }
}

const styleColumnTitleText = {
    textAlign: 'center',
    color: 'white',
    backgroundColor: 'dimgrey'
};

