import * as React from 'react';

export default class ColumnTitle extends React.Component<{ name: string, issueCount: number }> {
    public render() {

        return <h3 className="ColumnTitle">{this.props.name} ({this.props.issueCount})</h3>
    }
}