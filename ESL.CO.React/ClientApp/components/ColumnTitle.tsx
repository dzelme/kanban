import * as React from 'react';

export default class ColumnTitle extends React.Component<{ name: string, issueCount: number }> {
    public render() {

        return <h2 style={styleColumnTitleText}><strong>{this.props.name} ({this.props.issueCount})</strong></h2>
    }
}

const styleColumnTitleText = {
    textAlign: 'center',
};
