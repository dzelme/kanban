import * as React from 'react';

export default class ColumnTitle extends React.Component<{ name: string, issueCount: number }> {
    public render() {

        return <h2 className="ColumnTitle"><strong>{this.props.name} ({this.props.issueCount})</strong></h2>
    }
}