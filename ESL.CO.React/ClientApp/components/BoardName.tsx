import * as React from 'react';

export default class BoardName extends React.Component<{ name: string, fromCache: boolean, message: string }> {

    public render() {
        return <div>
            <h1 style={styleText}><strong>{this.props.name}</strong></h1>
            {this.props.fromCache ? <h4 style={styleText}>Dati no keša</h4> : ""}<h4 style={styleText}>{this.props.message}</h4>
        </div>
    }
}

const styleText = {
    color: 'white'
}