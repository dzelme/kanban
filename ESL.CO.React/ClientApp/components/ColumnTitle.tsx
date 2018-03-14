import * as React from 'react';

export default class ColumnTitle extends React.Component<{ name: string }> {
    public render() {
        return <h2>{this.props.name}</h2>
    }
}